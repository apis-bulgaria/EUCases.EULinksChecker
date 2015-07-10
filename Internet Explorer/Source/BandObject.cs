using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SHDocVw;
using System.Drawing;
using System.ComponentModel;
using EUCases;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Diagnostics;


namespace BandObjectsLib
{
    [Serializable]
    public class BandObject : UserControl, IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow, IInputObject//, IPersistStream
    {
        public static int LeftPos;
        public static int TopPos;

        private static List<object> s_ToolBarList = new List<object>();


        #region Transparency

        [DllImport("uxtheme", ExactSpelling = true)]
        public extern static Int32 DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackColor == Color.Transparent)
            {
                IntPtr hdc = e.Graphics.GetHdc();
                Rectangle rec = new Rectangle(e.ClipRectangle.Left,
                    e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
                DrawThemeParentBackground(this.Handle, hdc, ref rec);
                e.Graphics.ReleaseHdc(hdc);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        #endregion

        /// <summary>
        /// Reference to the host explorer.
        /// </summary>
        public WebBrowserClass Explorer;
        protected IInputObjectSite BandObjectSite;
        /// <summary>
        /// This event is fired after reference to hosting explorer is retreived and stored in Explorer property.
        /// </summary>
        public event EventHandler ExplorerAttached;

        public BandObject()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BandObject));
            this.SuspendLayout();
            // 
            // BandObject
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "BandObject";
            this.ResumeLayout(false);

        }

        public virtual void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref DESKBANDINFO dbi)
        {
            dbi.ptMinSize.X = MinimumSize.Width; //Constants.ToolBarMinWidth;
            dbi.ptMaxSize.X = MaximumSize.Width;//Constants.ToolBarMaxWidth;
            //dbi.ptIntegral.X = //Constants.ToolBarWidth;

            dbi.ptMinSize.Y = Constants.ToolBarHeight;
            dbi.ptMaxSize.Y = Constants.ToolBarHeight;
            //dbi.ptIntegral.Y = Constants.ToolBarHeight;
            
            //if ((dbi.dwMask & DBIM.TITLE) != 0)
            //{
            //    dbi.wszTitle = this.Title;
            //}
            dbi.dwMask &= ~DBIM.BKCOLOR;            

            // Set this flags to best feet to your desired toolbar layout behaviour
            dbi.dwModeFlags = DBIMF.NORMAL | DBIMF.VARIABLEHEIGHT | DBIMF.ALWAYSGRIPPER;            
        }


        /// <summary>
        /// Called by explorer when band object needs to be showed or hidden.
        /// </summary>
        /// <param name="fShow"></param>
        public virtual void ShowDW(bool fShow)
        {
            if (fShow)
                Show();
            else
                Hide();
        }

        /// <summary>
        /// Called by explorer when window is about to close.
        /// </summary>
        public virtual void CloseDW(UInt32 dwReserved)
        {
            Dispose(true);
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public virtual void ResizeBorderDW(IntPtr prcBorder, Object punkToolbarSite, bool fReserved) { }

        public virtual void GetWindow(out System.IntPtr phwnd)
        {
            phwnd = Handle;
        }

        public virtual void ContextSensitiveHelp(bool fEnterMode) { }

        #region SetSite

        public virtual void SetSite(object pUnkSite)
        {
            if (pUnkSite is IInputObjectSite)
            {
                _SetSite(pUnkSite);
            }
            else if (pUnkSite is InternetExplorer)
            {
                // this code is executed by the BHO in order to show the toolbar immediatly 
                // after the installation
                ShowToolBar(pUnkSite);
            }
        }
        
        private void _SetSite(Object pUnkSite)
        {
            if (BandObjectSite != null)
            {
                Marshal.ReleaseComObject(BandObjectSite);
            }

            if (Explorer != null)
            {
                Marshal.ReleaseComObject(Explorer);
                Explorer = null;
            }

            BandObjectSite = pUnkSite as IInputObjectSite;
            if (BandObjectSite != null)
            {
                //pUnkSite is a pointer to object that implements IOleWindowSite or something  similar
                //we need to get access to the top level object - explorer itself
                //to allows this explorer objects also implement IServiceProvider interface
                //(don't mix it with System.IServiceProvider!)
                //we get this interface and ask it to find WebBrowserApp
                _IServiceProvider sp = BandObjectSite as _IServiceProvider;
                Guid guid = ExplorerGUIDs.IID_IWebBrowserApp;
                Guid riid = ExplorerGUIDs.IID_IUnknown;

                try
                {
                    object w;
                    sp.QueryService(
                        ref guid,
                        ref riid,
                        out w);

                    //once we have interface to the COM object we can create RCW from it
                    Explorer = (WebBrowserClass)Marshal.CreateWrapperOfType(
                        w as IWebBrowser,
                        typeof(WebBrowserClass)
                        );

                    OnExplorerAttached(EventArgs.Empty);
                }
                catch (COMException)
                {
                    //we anticipate this exception in case our object instantiated 
                    //as a Desk Band. There is no web browser service available.                   
                }
            }
            else
            {
                //you can warn that BandObjectSite is null;
            }

        }

        private void ShowToolBar(object site)
        {
            try
            {
                InternetExplorer ie = site as InternetExplorer;
                if (ie == null) return;

                object pvaClsid = (object)(new Guid(Constants.ToolbarGUID).ToString("B"));
                object pvarSize = null;
                object pvarShowTrue = (object)true;
                ie.ShowBrowserBar(ref pvaClsid, ref pvarShowTrue, ref pvarSize);
                //remove BHO entry from registry
                //try
                //{
                //    Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects\{" + Constants.ToolbarGUID + "}");
                //}
                //catch(Exception ex)
                //{
                //    if(!EventLog.SourceExists("Internet Explorer"))
                //    {
                //        EventLog.WriteEntry("Internet Explorer", ex.Message);
                //    }
                //}
            }
            catch
            {
            }
        }
        #endregion

        public virtual void GetSite(ref Guid riid, out Object ppvSite)
        {
            ppvSite = BandObjectSite;
        }

        /// <summary>
        /// Called explorer when focus has to be chenged.
        /// </summary>
        public virtual void UIActivateIO(Int32 fActivate, ref MSG Msg)
        {
            if (fActivate != 0)
            {
                Control ctrl = GetNextControl(this, true);//first
                if (ModifierKeys == Keys.Shift)
                    ctrl = GetNextControl(ctrl, false);//last

                if (ctrl != null) ctrl.Select();
                this.Focus();
            }
        }

        public virtual Int32 HasFocusIO()
        {
            return this.ContainsFocus ? 0 : 1; //S_OK : S_FALSE;
        }


        [DllImport("user32.dll")]
        public static extern int TranslateMessage(ref MSG lpMsg);

        [DllImport("user32", EntryPoint = "DispatchMessage")]
        static extern bool DispatchMessage(ref MSG msg);


        /// <summary>
        /// Called by explorer to process keyboard events. Undersatands Tab and F6.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>S_OK if message was processed, S_FALSE otherwise.</returns>
        public virtual Int32 TranslateAcceleratorIO(ref MSG msg)
        {
            if (msg.message == 0x100)//WM_KEYDOWN
            {
                if ((uint)msg.wParam == (uint)Keys.Tab || (uint)msg.wParam == (uint)Keys.F6)//keys used by explorer to navigate from control to control
                {
                    if (SelectNextControl(
                            ActiveControl,
                            ModifierKeys == Keys.Shift ? false : true,
                            true,
                            true,
                            false)
                        )
                        return 0;//S_OK
                }
                else
                {
                    //This sends messages for the backspace, home and other keys.
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                    return 0;//S_OK
                }
            }
            return 1;//S_FALSE
        }

        /// <summary>
        /// Override this method to handle ExplorerAttached event.
        /// </summary>
        /// <param name="ea"></param>
        protected virtual void OnExplorerAttached(EventArgs ea)
        {
            AddToolbar();

            if (ExplorerAttached != null)
                ExplorerAttached(this, ea);
        }

        /// <summary>
        /// Notifies explorer of focus change.
        /// </summary>
        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            BandObjectSite.OnFocusChangeIS(this as IInputObject, 1);
        }
        /// <summary>
        /// Notifies explorer of focus change.
        /// </summary>
        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            if (ActiveControl == null)
                BandObjectSite.OnFocusChangeIS(this as IInputObject, 0);
        }



        #region Collection Management

        private void AddToolbar()
        {
            try
            {
                lock (s_ToolBarList)
                {
                    s_ToolBarList.Add(this);
                }
            }
            catch
            {
                // you can log the error here
            }

        }

        private void RemoveToolbar()
        {
            try
            {
                lock (s_ToolBarList)
                {
                    if (s_ToolBarList.Contains(this))
                        s_ToolBarList.Remove(this);
                }
            }
            catch
            {
                // you can log the error here
            }
        }

        /// <summary>
        /// You can use this collection to synchronize operation between all the 
        /// toolbar instances in the same process.
        /// All new tabs in IE7 and all new windows share the same process
        /// </summary>
        public List<object> ToolbarsCollection
        {
            get
            {
                return s_ToolBarList;
            }
        }

        #endregion

        #region Navigation Methods

        /// <summary>
        /// Navigate to the given url in the same window
        /// </summary>
        /// <param name="url"></param>
        public void Navigate(string url)
        {
            object flags = 0;
            object obj4 = null;
            object obj5 = null;
            object obj6 = null;
            this.Explorer.Navigate(url, ref flags, ref obj4, ref obj5, ref obj6);
        }

        /// <summary>
        /// Navigate to the given url in new window
        /// </summary>
        /// <param name="url"></param>
        public void NewWindow(string url)
        {
            object flags = 1; //new window
            object obj1 = null;
            object obj2 = null;
            object obj3 = null;
            object URL = url;
            this.Explorer.Navigate(url, ref flags, ref obj1, ref obj2, ref obj3);
        }

        #endregion

        #region IPersistStream Members

        //long IPersistStream.GetClassID(out Guid pClassID)
        //{
        //    pClassID = this.GetType().GUID;
        //    return 0;
        //}

        //long IPersistStream.IsDirty()
        //{
        //    return 1;
        //}

        //long IPersistStream.Load(IStream pStm)
        //{
        //    return 0;

        //}

        //long IPersistStream.Save(IStream pStm, bool fClearDirty)
        //{
        //    LeftPos = Left;
        //    TopPos = Top;
        //    return 0;
        //}

        //long IPersistStream.GetSizeMax(out UInt64 pcbSize)
        //{
        //    pcbSize = (UInt64)0x80004001;
        //    return (long)0x80004001;
        //}



        #endregion

    }

    ///
    /// ToolStripSystemRenderer that overrides OnRenderToolStripBorder to avoid painting the borders.
    ///
    internal class NoBorderToolStripRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) 
        {
            e.ToolStrip.BackColor = Color.Transparent;
        }
    }
}
