//#define test
#undef test
#define with_ShowToolTip
#define with_Dict
#undef comm_v1
#define comm_v2
//#undef comm_v2
#undef with_LinkHint//20150616
#undef with_SvcRef
#define with_async
#define with_cite
//#define test_cite
//#define with_LinkCache
//#define comm_v1
//#undef comm_v1
//this.DeleteAllComments();
//this.Application.ActiveDocument.DeleteAllComments();
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;//
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Diagnostics;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
//using System.Timers;
//using EUCases.Tools.Classes;
namespace EUCases.EULinksCheckerWordAddIn
{
    public partial class EULinksCheckerAddIn
    {
        /// <summary>
        /// Should we use Html or not; at present it prooved itself to be stable
        /// </summary>
        public static bool bUseHtml = true;
        /// <summary>
        /// LCID is the User Interface Language ID
        /// </summary>
        public static int LCID = RdLcid();//UiLang
        /// <summary>
        /// Default UILanguage short string
        /// </summary>
        public static string sUiLang = "en";
        /// <summary>
        /// Global flag to show if the add-in has already been initialized
        /// </summary>
        public static bool bEULinksCheckerAddInLangInitialized = false;
        /// <summary>
        /// Global reference to the Ribbon
        /// </summary>
        public static EULCRibbon TheRibbon = null;
        /// <summary>
        /// Global reference to the AddIn itself
        /// </summary>
        public static EULinksCheckerAddIn EUA = null;
        //private System.Timers.Timer timer = new System.Timers.Timer(1000);
        /// <summary>
        /// Read-only property to the Word.Application
        /// </summary>
        public Word.Application WordApp
        {
            get
            {
                return this.Application;
            }
        }
        /// <summary>
        /// Read-only property to the Word.Application.ActiveDocument
        /// </summary>
        public Word.Document CurrentDoc
        {
            get
            {
                return this.WordApp.ActiveDocument;
            }
        }
        /// <summary>
        /// The moment the last URL has been requested
        /// </summary>
        private int LastUrlMom = 0;
        /// <summary>
        /// The moment last ContextMenu has been requested
        /// </summary>
        private int LastCmMom = 0;
        /// <summary>
        /// The file name that was connected with the last ContextMenu requested
        /// </summary>
        private string LastCmFn = "";
#if with_Dict
        private System.Collections.Generic.Dictionary<string, int> Lang = new System.Collections.Generic.Dictionary<string, int>();
#endif//with_Dict
        /// <summary>
        /// The last URL that has been requested
        /// </summary>
        private string LastCalledUrl = "";
        #region Private String constants
        private const string
            rk_SW = "Software",
            rk_EUCases = "EUCases",
            rk_EULinksCheckerWordAddIn = "EULinksCheckerWordAddIn",
            rk_VLang = "LCID",
            rk_VUser = "User",
            //TermStyleName = "_EULCTerm",
            cm_HyperlinkContextMenu = "Hyperlink Context Menu",
            cm_InsertLink = "Insert Link",
            cm_Text = "Text",
            cm_RemoveLink = "Remove Link",
#if with_cite
            cm_ShortCite="Short cite",
            cm_LongCite="Long cite",
#endif//with_cite
#if with_ShowToolTip
 cm_ShowToolTip = "Show ToolTip",
#endif
 cm_DocumentsReferringToThisAct = "Documents referring to this act / provision:",
            cm_AllDocuments = "All documents",
            cm_EuL = "EU legislation",
            cm_EuCL = "EU case law",
            cm_NaL = "National Legislation",
            cm_NaCL = "National Caselaw",
            cm_DocumentsIndexedWithThisTerm = "Documents indexed with this term:",
            cm_Credentials = "Credentials",
            ci_InsertLink = cm_InsertLink,
            ci_RemoveLink = cm_RemoveLink,
#if with_cite
            ci_ShortCite=cm_ShortCite,
            ci_LongCite=cm_LongCite,
#endif//with_cite
#if with_ShowToolTip
 ci_ShowToolTip = cm_ShowToolTip,
#endif
 ci_DocumentsReferringToThisAct = cm_DocumentsReferringToThisAct,
            ci_LinksAllDocuments = ci_DocumentsReferringToThisAct + cm_AllDocuments,
            ci_LinksEuL = ci_DocumentsReferringToThisAct + cm_EuL,
            ci_LinksEuCL = ci_DocumentsReferringToThisAct + cm_EuCL,
            ci_LinksNaL = ci_DocumentsReferringToThisAct + cm_NaL,
            ci_LinksNaCL = ci_DocumentsReferringToThisAct + cm_NaCL,
            ci_DocumentsIndexedWithThisTerm = cm_DocumentsIndexedWithThisTerm,
            ci_TermsAllDocuments = ci_DocumentsIndexedWithThisTerm + cm_AllDocuments,
            ci_TermsEuL = ci_DocumentsIndexedWithThisTerm + cm_EuL,
            ci_TermsEuCL = ci_DocumentsIndexedWithThisTerm + cm_EuCL,
            ci_TermsNaL = ci_DocumentsIndexedWithThisTerm + cm_NaL,
            ci_TermsNaCL = ci_DocumentsIndexedWithThisTerm + cm_NaCL,
            ci_Credentials = cm_Credentials,
            cAll = "All",
            cNatCL = "NatCL",
            cNatL = "NatL",
            cEuCL = "EuCL",
            cEuL = "EuL",
            /*
            cu_TermNatCL = "http://invtest.apis.bg/natcl.html",//
            cu_LinkNatCL = "http://invtest.apis.bg/natcl.html",
            cu_TermNatL = "http://invtest.apis.bg/natleg.html",//
            cu_LinkNatL = "http://invtest.apis.bg/natleg.html",
            cu_TermEUCL = "http://invtest.apis.bg/eucl.html",//
            cu_LinkEUCL = "http://invtest.apis.bg/eucl.html",
            cu_TermEUL = "http://invtest.apis.bg/euleg.html",//
            cu_LinkEUL = "http://invtest.apis.bg/euleg.html",
            cu_TermAllDoc = "http://invtest.apis.bg/all.html",//
            cu_LinkAllDoc = "http://invtest.apis.bg/all.html",
            cu_Link = "http://invtest.apis.bg/link.html",
            */
            fn_TmpHtmlFromResp = @"\tmphtmlfromresp.html",
            cLimit = "100",
            cHtmExt = ".htm",
            cDocXFN = @"\EuLinksChecker",
            cDocXExt = ".docx";
        public const string
                fn_HtmlResNE = @"\htmlres",
                fn_HtmlRes = @"\htmlres.htm";
        #endregion
        #region Property accessors to the context-menu
#if test
        /*private Office.CommandBarPopup
            linkMenu = null,
            termMenu = null;
        private Office.CommandBarButton
            bLinkAllDoc = null,
            bLinkEULeg = null,
            bLinkEUCL = null,
            bLinkNatLeg = null,
            bLinkNatCL = null,
            bTermAllDoc = null,
            bTermEUCL = null,
            bTermEULeg = null,
            bTermNatLeg = null,
            bTermNatCL = null,
            bAddLink = null,
            bRemoveLink = null,
            bCredentials = null;*/
#endif
        private Office.CommandBar _HLCM
        {
            get
            {
                return (Office.CommandBar)Application.CommandBars[cm_HyperlinkContextMenu];
            }
        }
        private Office.CommandBarButton _InsertLink
        {
            get
            {
                return MkSMenu(cm_Text, Resources.Resource.cm_InsertLink, ci_InsertLink, bAddLink_Click);
            }

        }
        private Office.CommandBarButton _RemoveLink
        {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_RemoveLink, ci_RemoveLink, bRemoveLink_Click);
            }
        }
#if with_cite
        private Office.CommandBarButton _ShortCite
        {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_ShortCite, ci_ShortCite, bShortCite_Click);
            }
        }
        private Office.CommandBarButton _LongCite
        {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_LongCite, ci_LongCite, bLongCite_Click);
            }
        }
        /*
        private Office.CommandBarButton _LegShortCite
        {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_LegShortCite, ci_LegShortCite, bLegShortCite_Click);
            }
        }
        private Office.CommandBarButton _LegLongCite{
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_LegLongCite, ci_LegLongCite, bLegLongCite_Click);
            }
        }
        private Office.CommandBarButton _CaseShortCite{
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_CaseShortCite, ci_CaseShortCite, bCaseShortCite_Click);
            }
        }
        private Office.CommandBarButton _CaseLongCite {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_CaseLongCite, ci_CaseLongCite, bCaseLongCite_Click);
            }
        }
        */
#endif//with_cite
#if with_ShowToolTip
        private Office.CommandBarButton _ShowToolTip
        {
            get
            {
                return MkSMenu(_HLCM, Resources.Resource.cm_ShowToolTip, ci_ShowToolTip, bShowToolTip_Click);
            }
        }
#endif
        private Office.CommandBarPopup _linkMenu
        {
            get
            {
                Office.CommandBarPopup o = MkMenu(_HLCM, Resources.Resource.cm_DocumentsReferringToThisAct, ci_DocumentsReferringToThisAct);
                MkSMenu(o, Resources.Resource.cm_AllDocuments, ci_LinksAllDocuments, bLinkAllDoc_Click);
                MkSMenu(o, Resources.Resource.cm_EuL, ci_LinksEuL, bLinkEULeg_Click);
                MkSMenu(o, Resources.Resource.cm_EuCL, ci_LinksEuCL, bLinkEUCL_Click);
                MkSMenu(o, Resources.Resource.cm_NaL, ci_LinksNaL, bLinkNatLeg_Click);
                MkSMenu(o, Resources.Resource.cm_NaCL, ci_LinksNaCL, bLinkNatCL_Click);
                return o;
            }
        }
        private Office.CommandBarPopup _termMenu
        {
            get
            {
                Office.CommandBarPopup o = MkMenu(_HLCM, Resources.Resource.cm_DocumentsIndexedWithThisTerm, ci_DocumentsIndexedWithThisTerm);
                MkSMenu(o, Resources.Resource.cm_AllDocuments, ci_TermsAllDocuments, bTermAllDoc_Click);
                MkSMenu(o, Resources.Resource.cm_EuL, ci_TermsEuL, bTermEULeg_Click);
                MkSMenu(o, Resources.Resource.cm_EuCL, ci_TermsEuCL, bTermEUCL_Click);
                MkSMenu(o, Resources.Resource.cm_NaL, ci_TermsNaL, bTermNatLeg_Click);
                MkSMenu(o, Resources.Resource.cm_NaCL, ci_TermsNaCL, bTermNatCL_Click);
                return o;
            }
        }
#if test
        private void RemoveClickEvent(Office.CommandBarButton b)
        {
            //System.Reflection.FieldInfo f = typeof(Office.CommandBarButton).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            System.Reflection.FieldInfo f = typeof(Office.CommandBarButton).GetField("Click", BindingFlags.Static | BindingFlags.NonPublic);
            if (f == null) return;
            object o = f.GetValue(b);
            System.Reflection.PropertyInfo p = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList l = (EventHandlerList)p.GetValue(b, null);
            l.RemoveHandler(o, l[o]);

            /*FieldInfo f1 = typeof(Control).GetField("EventClick",
                BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);*/
        }
        /*private void MkNil()
        {
            linkMenu = null;
            termMenu = null;
            bLinkAllDoc = null;
            bLinkEULeg = null;
            bLinkEUCL = null;
            bLinkNatLeg = null;
            bLinkNatCL = null;
            bTermAllDoc = null;
            bTermEUCL = null;
            bTermEULeg = null;
            bTermNatLeg = null;
            bTermNatCL = null;
            bAddLink = null;
            bRemoveLink = null;
            bCredentials = null;
        }*/
#endif
        #endregion
#if with_Dict
        /// <summary>
        /// Set the current document encoding to Utf8
        /// </summary>
        public void SetUtf8()
        {
            CurrentDoc.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            WordApp.DefaultWebOptions().Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            WordApp.DefaultWebOptions().AlwaysSaveInDefaultEncoding = false;
        }
        /// <summary>
        /// Disable SpellChecker. Sometimes enabling it messes with the context-menu stuff
        /// </summary>
        public void SwitchOffSpellChecker()
        {
            try
            {
                WordApp.CheckLanguage = false;
                WordApp.Selection.NoProofing = 1;
            }
            catch { }
            /*
        Selection.LanguageID = wdEnglishUS;
        Selection.NoProofing = true;
        Application.CheckLanguage = false;//True
            */
        }
        /// <summary>
        /// Save document in DocX format
        /// </summary>
        public void SaveAsDocX()
        {
            string r = "", fn = FAFNX();
            object ofn = fn;
            Microsoft.Office.Interop.Word.WdSaveFormat ff = WdSaveFormat.wdFormatXMLDocument;
            object off = ff;
            Microsoft.Office.Core.MsoEncoding enc = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            object oenc = enc;
            var m = Type.Missing;
            WordApp.ActiveDocument.SaveAs2(FileName: ofn, FileFormat: off, Encoding: oenc, SaveAsAOCELetter: false, CompatibilityMode: 0, LockComments: false, EmbedTrueTypeFonts: false);
        }
        /// <summary>
        /// FirstAvailableFileName
        /// </summary>
        /// <param name="d">file name without extension</param>
        /// <param name="e">the extension</param>
        /// <param name="del">should we try to delete the file if found or not</param>
        /// <returns></returns>
        public string _FAFN(string d, string e,bool del)//FirstAvailableFileName of extension e
        {
            int i = 0;
            bool b = false;
            string
                f = d + e,
                cd = System.IO.Directory.GetCurrentDirectory(),
                r = cd + f;
            while (true)
            {
                if (System.IO.File.Exists(r))
                {
                    if (del)
                        try
                        {
                            System.IO.File.Delete(r);
                            b = true;
                        }
                        catch { b = false; }
                    else b = false;
                }
                else b = true;
                if (b) break;
                i++;
                r = cd + d + i.ToString() + e;
            }
            return r;
        }
        /// <summary>
        /// FirstAvailableFileName of name fn_HtmlResNE and extension cHtmExt; a delete operation is tried
        /// </summary>
        /// <returns></returns>
        public string FAFN()//FirstAvailableFileName
        {
            return _FAFN(fn_HtmlResNE, cHtmExt, true);
        }
        /// <summary>
        /// FirstAvailableFileName of name cDocXFN and extension cDocXExt; a delete operation is suppressed
        /// </summary>
        /// <returns></returns>
        public string FAFNX()
        {
            return _FAFN(cDocXFN, cDocXExt, false);
            /*int i = 0;
            bool b = false;
            string
                f = cDocXFN + cDocXExt,
                cd = System.IO.Directory.GetCurrentDirectory(),
                r = cd + f;
            while (true)
            {
                if (System.IO.File.Exists(r))
                {
                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog()
                    {
                        DefaultExt = cDocXExt,
                        Filter = "DocX files (*" + cDocXExt + ")|*" + cDocXExt
                    };
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        r = sfd.FileName;
                        continue;
                    }
                }
                else b = true;
                if (b) break;
                i++;
                r = cd + cDocXFN + i.ToString() + cDocXExt;
            }
            return r;*/
        }
        /// <summary>
        /// Set Application.ActiveWindow PrintVew
        /// </summary>
        public void SetPrintView()
        {
            //Application.ActiveWindow.View.ReadingLayout=Not ActiveWindow.View.ReadingLayout;
            Application.ActiveWindow.ActivePane.View.Type = WdViewType.wdPrintView;
        }
        /// <summary>
        /// Init the languages dictionary
        /// </summary>
        private void InitLang()
        {
            //Lang = new System.Collections.Generic.Dictionary<string, int>();
            Lang.Clear();
            Lang.Add("bg", 1);
            Lang.Add("de", 2);
            Lang.Add("fr", 3);
            Lang.Add("en", 4);
            Lang.Add("it", 5);
            Lang.Add("et", 6);
            Lang.Add("el", 7);
            Lang.Add("ga", 8);
            Lang.Add("lv", 9);
            Lang.Add("lt", 10);
            Lang.Add("hu", 11);
            Lang.Add("mt", 12);
            Lang.Add("da", 13);
            Lang.Add("cs", 14);
            Lang.Add("es", 15);
            Lang.Add("sr", 16);
            Lang.Add("hr", 17);
            Lang.Add("sv", 18);
            Lang.Add("fi", 19);
            Lang.Add("sl", 20);
            Lang.Add("sk", 21);
            Lang.Add("ro", 22);
            Lang.Add("pt", 23);
            Lang.Add("pl", 24);
            Lang.Add("nl", 25);
        }
        /// <summary>
        /// Init the resources according to the culture read from the registry
        /// </summary>
        public static void InitRes()
        {
            if (bEULinksCheckerAddInLangInitialized) return;
            int i = RdLcid();
            CultureInfo c = new CultureInfo(i);
            Resources.Resource.Culture = c;// c;
            bEULinksCheckerAddInLangInitialized = true;
        }
        /// <summary>
        /// Simple function which returns the LanguageId according to the ListIndex of the language combo box in the fCredentials form
        /// </summary>
        /// <param name="i">The ListIndex itself</param>
        /// <returns></returns>
        public static string Idx2sLang(int i)//i is fCredentials.cmbUiLang.ItemIndex
        {
            string s = "en";
            switch (i)
            {
                case 0: s = "bg"; break;//LCID=2
                case 1: s = "en"; break;//LCID=9
                case 2: s = "de"; break;//LCID=7
                case 3: s = "fr"; break;//LCID=12
                case 4: s = "it"; break;//LCID=16
            }
            return s;
        }
        /// <summary>
        /// Set the resources according to the interface Language choosen via fCredentials form
        /// </summary>
        /// <param name="i"></param>
        public static void SetRes(int i)
        {
            if (i == EULinksCheckerAddIn.LCID) return;
            MessageBox.Show(Resources.Resource.cMsgChgLng, Resources.Resource.cInformation);
            bEULinksCheckerAddInLangInitialized = false;
            sUiLang = Idx2sLang(i);
            CultureInfo c = sUiLang == "" ? CultureInfo.CurrentCulture : new CultureInfo(sUiLang);
            LCID = c.LCID;
            WrLcid();
            Resources.Resource.Culture = c;
            EULCRibbon.ApplLng(TheRibbon);
            bEULinksCheckerAddInLangInitialized = true;
        }
        /// <summary>
        /// Generic procedure to save integer value to the registry
        /// </summary>
        /// <param name="s">the key of the value in the corresponding to the application place in the registry</param>
        /// <param name="i">the value to be saved</param>
        public static void WrInt(string s, int i)
        {
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, true);
            if (kSW == null) return;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, true);
            if (kEUCases == null) kEUCases = kSW.CreateSubKey(rk_EUCases);
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, true);
            if (kEULinksCheckerWordAddIn == null) kEULinksCheckerWordAddIn = kEUCases.CreateSubKey(rk_EULinksCheckerWordAddIn);
            kEULinksCheckerWordAddIn.SetValue(s, i);
            kEULinksCheckerWordAddIn.Close();
            kEUCases.Close();
            kSW.Close();

        }
        /// <summary>
        /// Read integer value from the place from the registry belonging to the application
        /// </summary>
        /// <param name="s">the key name</param>
        /// <param name="dflt">the default value that sould be returned if no corresponding registry found</param>
        /// <returns></returns>
        public static int RdInt(string s, int dflt)
        {
            int i = dflt;
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, false);
            if (kSW == null) goto l0;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, false);
            if (kEUCases == null) goto l1;
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, false);
            if (kEULinksCheckerWordAddIn == null) goto l2;
            i = Convert.ToInt32(kEULinksCheckerWordAddIn.GetValue(s, -1));
            if (i <= 0) i = dflt;
            kEULinksCheckerWordAddIn.Close();
        l2: kEUCases.Close();
        l1: kSW.Close();
        l0: return i;
        }
        /*RdStr(string k,string dflt);WrStr(string k,string v)--ToDo*/
        /// <summary>
        /// Write LCID to the registry
        /// </summary>
        public static void WrLcid()
        {
            WrInt(rk_VLang, LCID);
            /*RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, true);
            if (kSW == null) return;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, true);
            if (kEUCases == null) kEUCases = kSW.CreateSubKey(rk_EUCases);
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, true);
            if (kEULinksCheckerWordAddIn == null) kEULinksCheckerWordAddIn = kEUCases.CreateSubKey(rk_EULinksCheckerWordAddIn);
            kEULinksCheckerWordAddIn.SetValue(rk_VLang, LCID);
            kEULinksCheckerWordAddIn.Close();
            kEUCases.Close();
            kSW.Close();*/
        }
        /// <summary>
        /// Read LCID from the registry
        /// </summary>
        /// <returns></returns>
        public static int RdLcid()
        {
            return RdInt(rk_VLang, CultureInfo.CurrentCulture.LCID);
            /*int i = CultureInfo.CurrentCulture.LCID;
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, false);
            if (kSW == null) goto l0;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, false);
            if (kEUCases == null) goto l1;
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, false);
            if (kEULinksCheckerWordAddIn == null) goto l2;
            i = Convert.ToInt32(kEULinksCheckerWordAddIn.GetValue(rk_VLang, -1));
            if (i <= 0) i = CultureInfo.CurrentCulture.LCID;
            kEULinksCheckerWordAddIn.Close();
        l2: kEUCases.Close();
        l1: kSW.Close();
        l0: return i;*/
        }
        /// <summary>
        /// Self explanatory
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        private static int LCID2LangId(int lcid)
        {
            int r = 4;// en default
            switch (lcid)
            {
                case 7: r = 2; break;//de
                case 9: r = 4; break;//en
                case 12: r = 3; break;//fr
                case 16: r = 5; break;//it
                case 2:
                case 1026: r = 1; break;//bg
            }
            return r;
        }
        /// <summary>
        /// Disable splash screen of the MsWord Application
        /// </summary>
        /// <returns></returns>
        public static bool DisableSplashScreen()
        {
            bool r = false;
            //'HKEY_CURRENT_USER\Software\Microsoft\Office\15.0\Common\General','DisableBootToOfficeStart'=(DWORD)1
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, true);
            if (kSW == null) return r;
            RegistryKey kMS = kSW.OpenSubKey("Microsoft", false);
            if (kMS == null) goto l1;
            RegistryKey kOff = kMS.OpenSubKey("Office", false);
            if (kOff == null) goto l2;
            RegistryKey k15 = kOff.OpenSubKey("15.0", false);
            //if (k15 == null) goto l3;
            if (k15 == null)
            {
                k15 = kOff.OpenSubKey("14.0", false);
                if (k15 == null)
                {
                    k15 = kOff.OpenSubKey("13.0", false);
                    if (k15 == null) goto l3;
                }
            }
            RegistryKey kCm = k15.OpenSubKey("Common", false);
            if (kCm == null) goto l4;
            RegistryKey kGe = kCm.OpenSubKey("General", true);
            if (kGe == null) goto l5;
            int i = 1;
            kGe.SetValue("DisableBootToOfficeStart", i);
            kGe.Close();
            r = true;
        l5:
            kCm.Close();
        l4:
            k15.Close();
        l3:
            kOff.Close();
        l2:
            kMS.Close();
        l1:
            kSW.Close();
            return r;
        }
        /// <summary>
        /// Tester for presence of links within the current selection
        /// </summary>
        /// <returns></returns>
        public bool HasLinkInSelection()
        {
            try
            {
                var i = CurrentDoc.Hyperlinks.GetEnumerator();
                int s = Application.Selection.Start, e = Application.Selection.End;
                i.Reset();
                while (i.MoveNext())
                {
                    Word.Hyperlink h = (Word.Hyperlink)i.Current;
                    try
                    {
                        if (h.Range.Start >= s && h.Range.End <= e) return true;
                    }
                    catch { }
                }
            }
            catch
            {
            }
            return false;
        }
        /// <summary>
        /// Public function returning the HyperText presentation of the current active document
        /// </summary>
        /// <returns></returns>
        public string sHtml()
        {
            /*Word.Document ad = CurrentDoc;
            string r = "", fn = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes;
            object ofn = fn;
            Microsoft.Office.Interop.Word.WdSaveFormat ff = WdSaveFormat.wdFormatHTML;
            object off = ff;

            Microsoft.Office.Core.MsoEncoding enc = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            object oenc = enc;
            var m = Type.Missing;
            ad.SaveAs2(ref ofn, ref off, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref oenc);
            r = F2S(fn);
            ad.Close();
            //System.IO.File.Delete(fn);
            return r;*/
            /**/
            /*Sub Macro3()
            '
             *     ActiveDocument.SaveAs2 FileName:="1114.htm", FileFormat:=wdFormatFilteredHTML _
                    , LockComments:=False, Password:="", AddToRecentFiles _
                    :=True, WritePassword:="", ReadOnlyRecommended:=False, EmbedTrueTypeFonts _
                    :=False, SaveNativePictureFormat:=False, SaveFormsData:=False, _
                    SaveAsAOCELetter:=False, CompatibilityMode:=0, Encoding:=msoEncodingUTF8, OptimizeForBrowser:=True, SaveAsAOCELetter:=False
                ActiveWindow.View.Type = wdWebView
            
             
            ' Macro3 Macro
            '
            '
                With ActiveDocument.WebOptions
                    .RelyOnCSS = True
                    .OptimizeForBrowser = True
                    .OrganizeInFolder = True
                    .UseLongFileNames = True
                    .RelyOnVML = False
                    .AllowPNG = True
                    .ScreenSize = msoScreenSize1024x768
                    .PixelsPerInch = 96
                    .Encoding = msoEncodingUTF8
                End With
                With Application.DefaultWebOptions
                    .UpdateLinksOnSave = True
                    .CheckIfOfficeIsHTMLEditor = True
                    .CheckIfWordIsDefaultHTMLEditor = True
                    .AlwaysSaveInDefaultEncoding = False
                    .SaveNewWebPagesAsWebArchives = True
                End With
            End Sub*/
            //System.Windows.Forms.MessageBox.Show("beg html src");
            Word.Document ad = CurrentDoc;
            int osb = ad.ActiveWindow.Selection.Start,
                ose = ad.ActiveWindow.Selection.End;
            ad.ActiveWindow.Selection.WholeStory();
            ad.ActiveWindow.Selection.Copy();
            Word.Document wd = WordApp.Documents.Add();
            wd.ActiveWindow.Visible = false;
            Word.Range rng = wd.Range();
            rng.PasteAndFormat(Word.WdRecoveryType.wdFormatOriginalFormatting);//wdFormatOriginalFormatting);//wdFormatPlainText,wdPasteDefault
            try
            {
                Clipboard.Clear();
            }
            catch { }
            try { Clipboard.SetData(System.Windows.Forms.DataFormats.Text, ""); }
            catch { }
            string r = "",
                //fn = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes
                fn = FAFN()
                ;
            object ofn = fn;
            Microsoft.Office.Interop.Word.WdSaveFormat ff = WdSaveFormat.wdFormatFilteredHTML;
            object off = ff;
            Microsoft.Office.Core.MsoEncoding enc = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            object oenc = enc;
            var m = Type.Missing;
            //wd.SaveAs2(ref ofn, ref off, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref m, ref oenc);
            wd.SaveAs2(FileName: ofn, FileFormat: off, Encoding: oenc, SaveAsAOCELetter: false, CompatibilityMode: 0, LockComments: false, EmbedTrueTypeFonts: false);
            wd.Close();
            wd = null;
            ad.Activate();
            ad.ActiveWindow.Selection.Start = osb;
            ad.ActiveWindow.Selection.End = ose;
            r = F2S(fn);
            System.IO.File.Delete(fn);
            //System.Windows.Forms.MessageBox.Show("end html src");
            return r;
            /**/
        }
        /// <summary>
        /// Process Indicator top-most form
        /// </summary>
        public EUCases.EULinksCheckerWordAddIn.Forms.frmTM fBusyTM = null;//fBusyInit();
        public EUCases.EULinksCheckerWordAddIn.Forms.fCite CITE = null;//fCiteInit();
        /// <summary>
        /// Initializer procedure for the Process indicator
        /// </summary>
        public void fBusyInit()
        {
            if (fBusyTM != null) return;
            fBusyTM = new EUCases.EULinksCheckerWordAddIn.Forms.frmTM();
            fBusyTM.Visible = false;
            //timer.Elapsed += OnTick;
            //timer.Enabled = true;
        }
        public void fCiteInit() { 
            if (CITE != null) return;
            CITE = new Forms.fCite();
            CITE.Visible = false;
        }
        /*private void OnTick(object source, ElapsedEventArgs e)
        {
            if (!fBusyTM.Visible) return;
            if (htmlPcsSuccess) 
                try
                {
                    fBusyTM.Visible = false;
                    fBusyTM.Hide();
                }
                catch { }
        }*/
        /// <summary>
        /// A procedure to show the ProcessIndicator
        /// </summary>
        public void ShowBusyTM()
        {
            //#define WM_SHOWWINDOW                   0x0018//24
            //Message.Create(fBusyTM.Handle, 24, (System.IntPtr)1, (System.IntPtr)0);
            try
            {
                fBusyTM.SetMsg(Resources.Resource.cWorking);
                fBusyTM.Show();
                fBusyTM.Refresh();
            }
            catch { }
        }
        /// <summary>
        /// A procedure to hide the ProcessIndicator
        /// </summary>
        public void HideBusyTM()
        {
            //Message.Create(fBusyTM.Handle, 24, (System.IntPtr)0, (System.IntPtr)0);
            try
            {
                fBusyTM.Visible = false;
                fBusyTM.Hide();
            }
            catch { }
        }
        /*public static string RdUser()
        {
            string s = "";
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, false);
            if (kSW == null) goto l0;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, false);
            if (kEUCases == null) goto l1;
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, false);
            if (kEULinksCheckerWordAddIn == null) goto l2;
            s = kEULinksCheckerWordAddIn.GetValue(rk_VUser, "").ToString();
            kEULinksCheckerWordAddIn.Close();
        l2: kEUCases.Close();
        l1: kSW.Close();
        l0: return s;
        }
        public static void WrUser(string s)
        {
            RegistryKey kSW = Registry.CurrentUser.OpenSubKey(rk_SW, true);
            if (kSW == null) return;
            RegistryKey kEUCases = kSW.OpenSubKey(rk_EUCases, true);
            if (kEUCases == null) kEUCases = kSW.CreateSubKey(rk_EUCases);
            RegistryKey kEULinksCheckerWordAddIn = kEUCases.OpenSubKey(rk_EULinksCheckerWordAddIn, true);
            if (kEULinksCheckerWordAddIn == null) kEULinksCheckerWordAddIn = kEUCases.CreateSubKey(rk_EULinksCheckerWordAddIn);
            kEULinksCheckerWordAddIn.SetValue(rk_VUser, s);
            kEULinksCheckerWordAddIn.Close();
            kEUCases.Close();
            kSW.Close();
        }*/
#endif//with_Dict
        /// <summary>
        /// An event handler fired on start-up
        /// </summary>
        /// <param name="sender">system prepared</param>
        /// <param name="e">system prepared</param>
        private void EULinksCheckerAddIn_Startup(object sender, System.EventArgs e)
        {
            EUA = this;
            //System.IO.File.Delete(WordApp.NormalTemplate.FullName);
            DisableSplashScreen();
            SetUtf8();
            fBusyInit();
            fCiteInit();
            //'HKEY_CURRENT_USER\Software\Microsoft\Office\15.0\Common\General','DisableBootToOfficeStart'=(DWORD)1
            InitRes();
#if with_Dict
            InitLang();
#endif//with_Dict
            for (int i = 0; i < 100; i++)
            {
                DelPopup(_linkMenu);
                DelPopup(_termMenu);
                _RemoveLink.Delete();
#if with_ShowToolTip
                _ShowToolTip.Delete();
#endif
            }
            //if (Application.Documents.Count == 0)return;
            Application.WindowBeforeRightClick +=
                new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(application_WindowBeforeRightClick);
            /*Application.WindowActivate += 
                new Microsoft.Office.Interop.Word.ApplicationEvents4_WindowActivateEventHandler(application_WindowActivate);
            Application.DocumentBeforeClose += 
                new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(application_DocumentBeforeClose);
            Application.WindowDeactivate += 
                new Microsoft.Office.Interop.Word.ApplicationEvents4_WindowDeactivateEventHandler(application_WindowDeactivate);
            Application.DocumentOpen += 
                new Word.ApplicationEvents4_DocumentOpenEventHandler(application_DocumentOpen);
            */
            Application.CustomizationContext = CurrentDoc;
            //            AddContextMenuItems();
        }
        /// <summary>
        /// Context-menu stuff
        /// </summary>
        #region Context-Menu stuff
        private bool SubMenuExists(string s, string tag)
        {
            Office.CommandBarButton f = (Office.CommandBarButton)Application.CommandBars[s].FindControl(Office.MsoControlType.msoControlButton, missing, tag, System.Type.Missing);
            return (f != null);
        }
        private bool SubMenuExists(Office.CommandBar o, string tag)
        {
            Office.CommandBarButton f = (Office.CommandBarButton)o.FindControl(Office.MsoControlType.msoControlButton, missing, tag, System.Type.Missing);
            return (f != null);
        }
        /// <summary>
        /// MakeSubMenu
        /// </summary>
        /// <param name="m">The menu tag</param>
        /// <param name="sm">The submenu caption</param>
        /// <param name="tag">The tag of the submenu created</param>
        /// <param name="e">The event handler</param>
        /// <returns>The newly created submenu</returns>
        private Office.CommandBarButton MkSMenu(string m, string sm, string tag, Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler e)
        {
            return MkSMenu(Application.CommandBars[m], sm, tag, e);
        }
        /// <summary>
        /// MakeSubMenu
        /// </summary>
        /// <param name="o">The parent menu object</param>
        /// <param name="sm">The submenu caption</param>
        /// <param name="tag">The tag of the submenu created</param>
        /// <param name="e"></param>
        /// <returns>The newly created submenu</returns>
        private Office.CommandBarButton MkSMenu(Office.CommandBar o, string sm, string tag, Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler e)
        {
            Office.CommandBarButton f = (Office.CommandBarButton)(o.FindControl(Office.MsoControlType.msoControlButton, missing, tag, System.Type.Missing));
            if (f != null)
            {
                f.Click -= e;
                f.Delete();
                f = null;
            }
            f = (Office.CommandBarButton)(o.Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true));
            f.Tag = tag;
            f.accName = f.Caption = sm;
#if test
            RemoveClickEvent(f);
#endif
            f.Click += e;
            return f;
        }
        private Office.CommandBarButton MkSMenu(Office.CommandBarPopup o, string sm, string tag, Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler e)
        {
            foreach (Office.CommandBarButton _ in o.Controls)
                if (_.Caption.Equals(sm) && _.Tag.Equals(tag))
                {
                    _.Click -= e;
                    _.Delete();
                }
            Office.CommandBarButton f;
            System.Collections.IEnumerator i = o.Controls.GetEnumerator();
            Office.CommandBarButton b = null, x;
            i.Reset();
            while (i.MoveNext())
            {
                x = (Office.CommandBarButton)i.Current;
                if (x.Caption.Equals(sm) && x.Tag.Equals(tag))
                {
                    b = x;
                    break;
                }
            }
            if (b == null)
            {
                f = (Office.CommandBarButton)o.Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
                f.Tag = tag;
                f.accName = f.Caption = sm;
#if test
                RemoveClickEvent(f);
#endif
                f.Click += e;
                return f;
            }
            else return b;
            /*try
            {
                f = (Office.CommandBarButton)o.Controls[sm];
            }
            catch
            {
                f = (Office.CommandBarButton)o.Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
                f.Tag = f.accName = f.Caption = sm;
                RemoveClickEvent(f);
                f.Click += e;
            }
            return f;*/
        }
        private Office.CommandBarPopup MenuExists(Office.CommandBar o, string tag)
        {
            Office.CommandBarPopup f = (Office.CommandBarPopup)o.FindControl(Office.MsoControlType.msoControlPopup, System.Type.Missing, tag, System.Type.Missing);
            return f;
        }
        /// <summary>
        /// Delete menu
        /// </summary>
        /// <param name="a">the menu to be deleted</param>
        private void DelPopup(Office.CommandBarPopup a)
        {
            foreach (Office.CommandBarButton b in a.Controls) b.Delete();
            foreach (Office.CommandBarPopup po in a.Controls) DelPopup(po);
            a.Delete();
        }
        /// <summary>
        /// another Make menu overload made for convinience
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private Office.CommandBarPopup MkMenu(Office.CommandBar o, string s, string tag)
        {
            Office.CommandBarPopup f = (Office.CommandBarPopup)o.FindControl(Office.MsoControlType.msoControlPopup, missing, tag, System.Type.Missing);
            if (f != null) DelPopup(f);
            f = (Office.CommandBarPopup)o.Controls.Add(Office.MsoControlType.msoControlPopup, missing, missing, 1, true);
            f.Tag = tag;
            f.accName = f.Caption = s;
            return f;
        }
        #endregion
#if test
        /*public void AddContextMenuItems()
        {
            //StringBuilder sb = new StringBuilder();
            //for (int i = 1; i <= Application.CommandBars.Count; i++)
            //{
            //    sb.AppendLine(Application.CommandBars[i].Name);
            //}

            Office.CommandBar commandBar = Application.CommandBars[cm_HyperlinkContextMenu];
            //System.Windows.Forms.MessageBox.Show(commandBar.Name);
            bRemoveLink = MkSMenu(commandBar, cm_RemoveLink, bRemoveLink_Click);
            bShowToolTip= MkSMenu(commandBar, cm_RemoveLink, bShowToolTip_Click);
            /*if (!SubMenuExists(commandBar, cm_RemoveLink))
            {
                bRemoveLink = (Office.CommandBarButton)commandBar.Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
                bRemoveLink.accName = //cm_RemoveLink;
                bRemoveLink.Tag = //cm_RemoveLink;
                bRemoveLink.Caption = cm_RemoveLink;
                bRemoveLink.Click += bRemoveLink_Click;
            }*/

        //            linkMenu = MkMenu(commandBar, cm_DocumentsReferringToThisAct);
        /*Office.CommandBarPopup linkMenu;
        linkMenu = MenuExists(commandBar, cm_DocumentsReferringToThisAct);
        if(linkMenu==null){
            linkMenu= (Office.CommandBarPopup)commandBar.Controls.Add(Office.MsoControlType.msoControlPopup, missing, missing, 1, true);
            linkMenu.Caption = //cm_DocumentsReferringToThisAct;
            linkMenu.Tag = cm_DocumentsReferringToThisAct;
        }*/

        //            bLinkAllDoc = MkSMenu(linkMenu, cm_AllDocuments, bLinkAllDoc_Click);
        /*bLinkAllDoc = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bLinkAllDoc.accName = //cm_AllDocuments;
        bLinkAllDoc.Caption = //cm_AllDocuments;
        bLinkAllDoc.Tag = cm_AllDocuments;
        bLinkAllDoc.Click += bLinkAllDoc_Click;*/

        //            bLinkEULeg = MkSMenu(linkMenu, cm_EuL, bLinkEULeg_Click);
        /*bLinkEULeg = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bLinkEULeg.accName = //cm_EuL;
        bLinkEULeg.Caption = //cm_EuL;
        bLinkEULeg.Tag = cm_EuL;
        bLinkEULeg.Click += bLinkEULeg_Click;*/

        //            bLinkEUCL = MkSMenu(linkMenu, cm_EuCL, bLinkEUCL_Click);
        /*bLinkEUCL = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bLinkEUCL.accName = //cm_EuCL;
        bLinkEUCL.Caption = //cm_EuCL;
        bLinkEUCL.Tag = cm_EuCL;
        bLinkEUCL.Click += bLinkEUCL_Click;*/

        //            bLinkNatLeg = MkSMenu(linkMenu, cm_NaL, bLinkNatLeg_Click);
        /*bLinkNatLeg = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bLinkNatLeg.accName = //cm_NaL;
        bLinkNatLeg.Caption = //cm_NaL;
        bLinkNatLeg.Tag = cm_NaL;
        bLinkNatLeg.Click += bLinkNatLeg_Click;*/

        //            bLinkNatCL = MkSMenu(linkMenu, cm_NaCL, bLinkNatCL_Click);
        /*bLinkNatCL = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bLinkNatCL.accName = //cm_NaCL;
        bLinkNatCL.Caption = //cm_NaCL;
        bLinkNatCL.Tag = cm_NaCL;
        bLinkNatCL.Click += bLinkNatCL_Click;*/

        //            Office.CommandBarPopup termMenu = MkMenu(commandBar, cm_DocumentsIndexedWithThisTerm);
        /*var termMenu = (Office.CommandBarPopup)commandBar.Controls.Add(Office.MsoControlType.msoControlPopup, missing, missing, 1, true);
        termMenu.Caption = cm_DocumentsIndexedWithThisTerm;*/

        //            bTermAllDoc = MkSMenu(termMenu, cm_AllDocuments, bTermAllDoc_Click);
        /*bTermAllDoc = (Office.CommandBarButton)termMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bTermAllDoc.accName = //cm_AllDocuments;
        bTermAllDoc.Caption = //cm_AllDocuments;
        bTermAllDoc.Tag = cm_AllDocuments;
        bTermAllDoc.Click += bTermAllDoc_Click;*/

        //            bTermEULeg = MkSMenu(termMenu, cm_EuL, bTermEULeg_Click);
        /*bTermEULeg = (Office.CommandBarButton)termMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bTermEULeg.accName = //cm_EuL;
        bTermEULeg.Caption = //cm_EuL;
        bTermEULeg.Tag = cm_EuL;
        bTermEULeg.Click += bTermEULeg_Click;*/

        //            bTermEUCL = MkSMenu(termMenu, cm_EuCL, bTermEUCL_Click);
        /*bTermEUCL = (Office.CommandBarButton)termMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bTermEUCL.accName = //cm_EuCL;
        bTermEUCL.Caption = //cm_EuCL;
        bTermEUCL.Tag = cm_EuCL;
        bTermEUCL.Click += bTermEUCL_Click;*/

        //            bTermNatLeg = MkSMenu(termMenu, cm_NaL, bTermNatLeg_Click);
        /*bTermNatLeg = (Office.CommandBarButton)termMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bTermNatLeg.accName = //cm_NaL;
        bTermNatLeg.Caption = cm_NaL;
        bTermNatLeg.Tag = cm_AllDocuments;
        bTermNatLeg.Click += bTermNatLeg_Click;*/

        //            bTermNatCL = MkSMenu(termMenu, cm_NaCL, bTermNatCL_Click);
        /*bTermNatCL = (Office.CommandBarButton)termMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        bTermNatCL.accName = //cm_NaCL;
        bTermNatCL.Caption = //cm_NaCL;
        bTermNatCL.Tag = cm_NaCL;
        bTermNatCL.Click += bTermNatCL_Click;*/

        //            Office.CommandBar commandBar2 = Application.CommandBars[cm_Text];
        //System.Windows.Forms.MessageBox.Show(commandBar.Name);

        //            bAddLink = MkSMenu(commandBar2, cm_InsertLink, bAddLink_Click);
        //            bAddLink.Visible = true;

        /*bAddLink = (Office.CommandBarButton)commandBar2.Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
        bAddLink.accName = //cm_InsertLink;
        bAddLink.Tag = //cm_InsertLink;
        bAddLink.Caption = cm_InsertLink;
        bAddLink.Visible = true;
        bAddLink.Click += bAddLink_Click;*/


        //bCredentials = (Office.CommandBarButton)linkMenu.Controls.Add(Office.MsoControlType.msoControlButton);
        //bCredentials.accName = cm_Credentials;
        //bCredentials.Caption = cm_Credentials;
        //bCredentials.Tag = cm_Credentials;
        //bCredentials.Click += bCredentials_Click;

        //        }*/
#endif
        /// <summary>
        /// OnClick handlers
        /// </summary>
        #region OnClickStuff
        void bCredentials_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bCredentials_Click;
            EditCredentials();
        }
        void bRemoveLink_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bRemoveLink_Click;
            Remove1();
        }
#if with_cite
        void bShortCite_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bRemoveLink_Click;
            ShortCite();
        }
        void bLongCite_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bRemoveLink_Click;
            LongCite();
        }
#endif//with_cite
#if with_ShowToolTip
        void bShowToolTip_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bShowToolTip_Click;
            ShowToolTip();
        }
#endif
        void bAddLink_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Ctrl.Click -= bAddLink_Click;
            AddNewLink();
        }
        private void deburn(Office.CommandBarButton b, ref bool a, Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler e)
        {
            a = true;
            bool l = true;
            int i = 100;
            while (l && i > 1)
            {
                try
                {
                    b.Click -= e;
                    i--;
                }
                catch { l = false; }
            }
            try { b.Delete(); }
            catch { }
        }
        private string ltUrl(int i, string s)
        {
            rUrl a = GetRUrl();
            if (a == null) return "";
            switch (i)
            {
                case 0: return a.LinksUrl(s); //break;
                case 1: return a.TermsUrl(s); //break;
                default: return "";
            }
        }
        private void OU(int mode, string u, Office.CommandBarButton Ctrl, ref bool CancelDefault, Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler e)
        {
            deburn(Ctrl, ref CancelDefault, bTermNatCL_Click);
            u = ltUrl(mode, u);
            if (u == "") return;
            int x = Environment.TickCount;
            if (x - LastUrlMom < 3000 && LastCalledUrl == u) return;
            LastUrlMom = x;
            LastCalledUrl = u;
            Process.Start(u);
            /*switch (mode)
            {
                case 0:Process.Start(u);break;
                case 1:ShowStrInBrowser(u);break;
            }*/
            //deburn(Ctrl, ref CancelDefault, bTermNatCL_Click);
        }
        void bTermNatCL_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(1, cNatCL, Ctrl, ref CancelDefault, bTermNatCL_Click);
        }
        void bTermNatLeg_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(1, cNatL, Ctrl, ref CancelDefault, bTermNatLeg_Click);
        }
        void bTermEUCL_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(1, cEuCL, Ctrl, ref CancelDefault, bTermEUCL_Click);
        }
        void bTermEULeg_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(1, cEuL, Ctrl, ref CancelDefault, bTermEULeg_Click);
        }
        void bTermAllDoc_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(1, cAll, Ctrl, ref CancelDefault, bTermAllDoc_Click);
        }
        void bLinkNatCL_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(0, cNatCL, Ctrl, ref CancelDefault, bLinkNatCL_Click);
        }
        void bLinkNatLeg_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(0, cNatL, Ctrl, ref CancelDefault, bLinkNatLeg_Click);
        }
        void bLinkEUCL_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(0, cEuCL, Ctrl, ref CancelDefault, bLinkEUCL_Click);
        }
        void bLinkEULeg_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(0, cEuL, Ctrl, ref CancelDefault, bLinkEULeg_Click);
        }
        void bLinkAllDoc_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            OU(0, cAll, Ctrl, ref CancelDefault, bLinkAllDoc_Click);
        }
        #endregion
#if test
        /*
        protected void application_DocumentOpen(Word.Document doc)
        {
            Application.CustomizationContext = Application.ActiveDocument;
            AddContextMenuItems();
        }
        protected void application_DocumentBeforeClose(Word.Document doc, ref bool Cancel)
        {
            MkNil();
        }
        protected void application_WindowActivate(Word.Document doc, Word.Window Wn)
        {
            Application.CustomizationContext = Application.ActiveDocument;
            AddContextMenuItems();
        }
        protected void application_WindowDeactivate(Word.Document doc, Word.Window Wn)
        {
            MkNil();
        }*/
#endif
        /// <summary>
        /// Important procedure to prepare the context-menus associated with selection
        /// </summary>
        public void application_WindowBeforeRightClick(Word.Selection selection, ref bool Cancel)
        {
            /**/
            int x = Environment.TickCount;
            string fn = CurrentDoc.FullName;
            if (x - LastCmMom < 100) if (fn == LastCmFn) return;
            LastCmMom = x;
            LastCmFn = fn;
            Application.CustomizationContext = CurrentDoc;
            //AddContextMenuItems();
            bool HasSelection = Application.Selection.End > Application.Selection.Start;
            //SetCommandVisibility(cm_InsertLink, HasSelection, cm_Text);
            var hl = GetCurrentHyperlink();
            rUrl r = GetRUrl(hl);
            bool IsHL = hl != null;
#if with_cite
            _ShortCite.Visible = IsHL && !r.IsTerm;
            _LongCite.Visible = IsHL && !r.IsTerm;
#endif//with_cite
            _InsertLink.Visible = HasSelection && !IsHL;
            _RemoveLink.Visible = IsHL;
#if with_ShowToolTip
            _ShowToolTip.Visible =
#endif
 _linkMenu.Visible = IsHL && r.IsLink;//IsHL;
            _termMenu.Visible = IsHL && r.IsTerm;//IsHL;
            //if (termMenu != null) termMenu.Visible = IsHL;
            //SetCommandVisibility(cm_DocumentsIndexedWithThisTerm, IsHL, cm_HyperlinkContextMenu);
            /*
            if (hl == null)
                return;
            //            if (hl.Address.Contains(cu_Link))
            SetCommandVisibility(cm_DocumentsReferringToThisAct, true, cm_HyperlinkContextMenu);
            //            if (hl.Address.Contains("http://invtest.apis.bg/term.html"))
            SetCommandVisibility(cm_DocumentsIndexedWithThisTerm, true, cm_HyperlinkContextMenu);
            //            if (hl.Address.Contains("invtest.apis.bg"))
            SetCommandVisibility(cm_RemoveLink, true, cm_HyperlinkContextMenu);
            */
        }
        /// <summary>
        /// Visibility controller for the context menus
        /// </summary>
        private void SetCommandVisibility(string name, bool visible, string commandBarName, string tag)
        {
            Application.CustomizationContext = Application.ActiveDocument;
            if (!SubMenuExists(commandBarName, tag))
                return;
            /*
            Office.CommandBar commandBar = Application.CommandBars[commandBarName];
            //if (SubMenuExists(commandBar, name))
            commandBar.Controls[name].Visible = visible;
            */
            Application.CommandBars[commandBarName].Controls[name].Visible = visible;
            //try{
            //commandBar.Controls[name].Visible = visible;
            //}catch {}//(Exception e) { System.Windows.Forms.MessageBox.Show(e.Message + " on name='" + name + "; commandBarName='" + commandBarName+"'"); }
        }
        /// <summary>
        /// A code executed on shutting down
        /// </summary>
        private void EULinksCheckerAddIn_Shutdown(object sender, System.EventArgs e)
        {
            DelPopup(_linkMenu);
            DelPopup(_termMenu);
            _RemoveLink.Delete();
#if with_ShowToolTip
            _ShowToolTip.Delete();
#endif
            //_InsertLink.Delete();
        }
        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(EULinksCheckerAddIn_Startup);
            this.Shutdown += new System.EventHandler(EULinksCheckerAddIn_Shutdown);
        }
        #endregion
        /// <summary>
        /// RemoveLinksAndTerms
        /// </summary>
        public void Test1()
        {
            RemoveLinksAndTerms();
        }
        /// <summary>
        /// Self explanatory
        /// </summary>
        /// <returns></returns>
        public bool CanRemoveLinksAndTerms()
        {
            List<Word.Hyperlink> hls = new List<Word.Hyperlink>();
            var i = CurrentDoc.Hyperlinks.GetEnumerator();
            i.Reset();
            while (i.MoveNext()) hls.Add((Word.Hyperlink)i.Current);
            return hls.Count > 0;
        }
        /// <summary>
        /// proceeds with links and terms removal
        /// </summary>
        /// <returns>true on success, false on error</returns>
        public bool RemoveLinksAndTerms()
        {
            List<Word.Hyperlink> hls = new List<Word.Hyperlink>();
            var i = CurrentDoc.Hyperlinks.GetEnumerator();
            i.Reset();
            while (i.MoveNext()) hls.Add((Word.Hyperlink)i.Current);
            foreach (var h in hls) Delete1Hl(h);
            CurrentDoc.Content.Find.Execute();//?
            return true;
        }
#if with_Dict
        /// <summary>
        /// convertor between languages of the document and the system culture
        /// </summary>
        /// <returns></returns>
        public string l2lid(string l)
        {
            l = l.ToLower();
            if (Lang.ContainsKey(l)) return Lang[l].ToString();
            else return "4";//en
        }
#else
        private static string l2lid(string l)
        {
            l = l.ToLower();
            return l == "bg" ? "1" : l == "de" ? "2" : l == "fr" ? "3" : l == "en" ? "4" : l == "it" ? "5" : l == "et" ? "6" : l == "el" ? "7" : l == "ga" ? "8" : l == "lv" ? "9" : l == "lt" ? "10" : l == "hu" ? "11" : l == "mt" ? "12" : l == "da" ? "13" : l == "cs" ? "14" : l == "es" ? "15" : l == "sr" ? "16" : l == "hr" ? "17" : l == "sv" ? "18" : l == "fi" ? "19" : l == "sl" ? "20" : l == "sk" ? "21" : l == "ro" ? "22" : l == "pt" ? "23" : l == "pl" ? "24" : l == "nl" ? "25" : "4";
        }
#endif//with_Dict
#if with_LinkCache
        private System.Collections.Generic.Dictionary<Word.Hyperlink, rUrl> LinkCache = new System.Collections.Generic.Dictionary<Word.Hyperlink, rUrl>();
#endif//with_LinkCache
        /// <summary>
        /// intermediate class to cope with the links
        /// </summary>
        public class rUrl
        {
            public string
                txt,
                url1,
                url,
                query,
                celex,
                lang,
                langid,
                uilang,
                art,
                xmlid;
            public bool IsLink { get { return celex != "" && xmlid == ""; } }
            public bool IsTerm { get { return celex == "" && xmlid != ""; } }
            public rUrl() { }
            public rUrl(string _txt, string _url1, string _url, string _query, string _celex, string _lang, string _langid, string _art)
            {
                txt = _txt;
                url1 = _url1;
                url = _url;
                query = _query;
                celex = _celex;
                lang = _lang;
                langid = _langid;
                art = _art;
            }
#if with_SvcRef
            public rUrl(EUCases.EULinksCheckerWordAddIn.ServiceReference.LinkTextPos x, string _langid)
            {
                celex = x.Celex;
                lang = x.Language;
                langid = _langid;
                art = x.ReferrenceInfo;
                MkUrl();
                //url = "http://app.eurocases.eu/api/Doc/ParHint/" + langid + "/" + celex;
                //if (art != "") url += "/" + art;
                url1 = "http://eur-lex.europa.eu/legal-content/" + lang + "/TXT/?uri=CELEX:" + celex;
                query = UrlQuery(url);
            }
#endif//with_SvcRef
            public void MkUrl()
            {
                url = "http://app.eurocases.eu/api/Doc/ParHint/" + langid + "/" + LCID2LangId(LCID).ToString() + "/" + celex;
                if (art != null) if (art != "") url += "/" + art;
            }
            public string LinksUrl(string domain)
            {
                //                string s = "http://app.eurocases.eu/api/Doc/DocInLinks/" + domain + "/" + langid + "/" + cLimit + "/" + celex;
                string s = "http://app.eurocases.eu/api/Doc/DocInLinks/" + domain + "/" + langid + "/" + LCID2LangId(LCID).ToString() + "/" + cLimit + "/" + celex;
                if (art != "") s += "/" + art;
                return s;
            }
            public string TermsUrl(string domain)
            {
                /*
Тази заявка работи с GET:
api/Doc/SearchByXmlId/{xmlId}/{domain}/{langId}/{siteLangId}
{xmlId} - това е новия параметър
Пример:
http://app.eurocases.eu/api/Doc/SearchByXmlId/64/all/1/1
string u="http://app.eurocases.eu/api/Doc/SearchByXmlId/"+xmlid+"/"+domain+"/"+langid+"/"+LCID2LangId(LCID).ToString()
                 */
                //string s = "http://app.eurocases.eu/api/Doc/" + domain + "/SearchByTerm/" + System.Web.HttpUtility.UrlEncode(srch) + "/" + langid;
                //return "http://app.eurocases.eu/api/Doc/SearchByTerm/" + System.Web.HttpUtility.UrlEncode(txt) + "/" + langid;
                /*++++++++
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    return client.UploadString("http://app.eurocases.eu/api/Doc/SearchByTerm/" + domain + "/" + langid + "/" + LCID2LangId(LCID).ToString() + "/", "=" + txt);
                }
                +++*/
                return "http://app.eurocases.eu/api/Doc/SearchByXmlId/" + xmlid + "/" + domain + "/" + langid + "/" + LCID2LangId(LCID).ToString();
            }
        };
#if with_SvcRef
        public rUrl GetRUrl(EUCases.EULinksCheckerWordAddIn.ServiceReference.LinkTextPos x)
        {
            return new rUrl(x, l2lid(x.Language));
        }
#endif//with_SvcRef
        /// <summary>
        /// Simple repeater of a hyperlink data into rUrl object
        /// </summary>
        /// <param name="h">the hyperlink</param>
        /// <returns>the rUrl object</returns>
        public rUrl GetRUrl(Word.Hyperlink h)
        {
            if (h == null) return null;
            rUrl a = new rUrl();
            a.txt = h.TextToDisplay;
            a.url1 = h.Address;
            a.lang = UrlLang(a.url1);
            a.langid = l2lid(a.lang);
            a.query = UrlQuery(a.url1);
            a.celex = UrlQueryCelex(a.query, "uri=CELEX");
            a.art = h.SubAddress;
            a.xmlid = UrlXmlId(a.query);
            a.MkUrl();
            return a;
        }
        /// <summary>
        /// rUrl construct from the current hyperlink
        /// </summary>
        /// <returns></returns>
        private rUrl GetRUrl()
        {
            return GetRUrl(GetCurrentHyperlink());
#if with_LinkCache
            if (LinkCache.ContainsKey(h)) return null;
            return LinkCache[h];
#endif//with_LinkCache
        }
        /// <summary>
        /// helper for the hint/tooltip functionality
        /// </summary>
        public class HintObject
        {
            public string HtmlContent { get; set; }
        }
        /// <summary>
        /// Remove tags from the html source string
        /// </summary>
        /// <param name="source">the input Html string</param>
        /// <returns>The plain text</returns>
        private string StripHtml(string source)
        {
            string o = Regex.Replace(source, "<[^>]*>", string.Empty);
            //get rid of multiple blank lines
            o = Regex.Replace(o, @"^\s*$\n", string.Empty, RegexOptions.Multiline);
            o = Regex.Replace(o, @"&lt;", "<", RegexOptions.Multiline);
            o = Regex.Replace(o, @"&gt;", ">", RegexOptions.Multiline);
            o = Regex.Replace(o, @"&nbsp;", " ", RegexOptions.Multiline);
            o = Regex.Replace(o, "\n;", " ", RegexOptions.Multiline);
            o = Regex.Replace(o, "\r;", " ", RegexOptions.Multiline);
            o = Regex.Replace(o, "\t;", " ", RegexOptions.Multiline);
            o = o.Length > 500 ? o.Substring(0, 500) : o;
            return o;
        }
        /*private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            return text;
        }*/
        /// <summary>
        /// Get document hint/tooltip
        /// </summary>
        /// <param name="lang">short string language</param>
        /// <param name="celex">CELEX number of the document</param>
        /// <param name="rf">Ref; an anchor within the document body, referenced by its CELEX; not mandatory</param>
        /// <returns>The doc hint/tooltip</returns>
        public string GetDocHint(string lang, string celex, string rf)
        {
            //http://app.eurocases.eu/api/Doc/ParHint/{LangId}/{docNumber}/{toPar}
            string u = "http://app.eurocases.eu/api/Doc/ParHint/" + l2lid(lang) + "/" + LCID2LangId(LCID).ToString() + "/" + celex + ((rf == "") ? "" : "/" + rf), c = "";
            if (u == "") return "";
            int x = Environment.TickCount;
            if (x - LastUrlMom < 3000 && LastCalledUrl == u) return "";
            LastUrlMom = x;
            LastCalledUrl = u;
            HttpWebRequest q = (HttpWebRequest)WebRequest.Create(u);
            try
            {
                q.Method = "GET";
                q.Timeout = 2000;
                q.ContentType = "text/plain";
                q.Accept = "application/json";
                HttpWebResponse r = (HttpWebResponse)q.GetResponse();
                var s = new StreamReader(r.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                c = s.ToString();
            }
            catch (Exception e)
            {
                c = "Exception fetching the hint: " + e.Message;
            }
            return c;
        }
        /// <summary>
        /// The Url Query part of the Url in s
        /// </summary>
        /// <param name="s">the whole url (accepts the part after the protocola and the server as well)</param>
        /// <returns></returns>
        private static string UrlQuery(string s)
        {
            int i = s.IndexOf("?");
            if (i == 0) return "";
            return s.Substring(++i);
        }
        /// <summary>
        /// Extract the value of the parameter named by n from the query string s
        /// </summary>
        /// <param name="s">The query string</param>
        /// <param name="n">The name of the parameter</param>
        /// <returns>the value of the parameter</returns>
        private static string UrlQueryParam(string s, string n)
        {
            string[] p, x = s.Split("&".ToCharArray());
            for (int i = 0; i < x.Length; i++)
            {
                p = x[i].Split("=".ToCharArray());
                if (p[0] == n) return p[1];
            }
            return "";
        }
        /// <summary>
        /// The CELEX number coded within a query string of a Url
        /// </summary>
        /// <param name="s">the query string</param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string UrlQueryCelex(string s, string n)
        {
            string[] p, x = s.Split("&".ToCharArray());
            for (int i = 0; i < x.Length; i++)
            {
                p = x[i].Split(":".ToCharArray());
                if (p[0] == n) return p[1];
            }
            return "";
        }
        private static string UrlXmlId(string s)
        {
            if (s.IndexOf("SearchByXmlId") >= 0)
                return UrlRewriteParam(s, 4);
            else return "";
        }
        private static string UrlRewriteParam(string s, int i)
        {
            s = s.ToLower();
            int j = s.IndexOf("://");
            if (j >= 0) s = s.Substring(j + 3);
            string[] p = s.Split("/".ToCharArray());
            if (i < p.Length) return s.Split("/".ToCharArray())[i];
            else return "";
        }
        /// <summary>
        /// language of a document represented by its Url
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string UrlLang(string s)
        {
            return UrlRewriteParam(s, 2);
            /*s = s.ToLower();
            int j = 0, i = s.IndexOf("://");
            s = s.Substring(i + 3);
            string[] p = s.Split("/".ToCharArray());
            return p[2];*/
        }
#if with_ShowToolTip
        /// <summary>
        /// Show tooltip over a link in a document
        /// </summary>
        private void ShowToolTip()
        {
            rUrl a = GetRUrl();
            if (a == null) return;
            string u = a.url;
            if (u == "") return;
            int x = Environment.TickCount;
            if (x - LastUrlMom < 3000 && LastCalledUrl == u) return;
            LastUrlMom = x;
            LastCalledUrl = u;
            Process.Start(u);
        }
#endif
#if !with_async
        public bool PutLinksAndTermsHtml(string html, ref string o)
        {
            Application.System.Cursor = Word.WdCursorType.wdCursorWait;
            SetUtf8();
#if with_SvcRef
            o = html;
            //            S2F(@"c:\orig-html.html", o);
            ServiceReference.LinkingServiceClient cl = new ServiceReference.LinkingServiceClient();
            ServiceReference.TextPosWrapper res_cl = cl.PutLinksText(html);
            foreach (ServiceReference.LinkTextPos a in res_cl.LinksTextPos)
            {
                int pos = a.Pos,
                    len = a.Length,
                    pl = pos + len;
#if with_LinkHint
                string id = a.Celex,
                    doc_hint = GetDocHint(a.Language, a.Celex, a.ReferrenceInfo),
                    plain = StripHtml(doc_hint),//20150616
                    plainNQ = plain.Replace("\"", ""),//20150616
                    eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.Language + "/TXT/?uri=CELEX:" + id;
                if (a.ReferrenceInfo != "") eurlex_uri += "#" + a.ReferrenceInfo;//"#"?
                //string first = o.Substring(pos), c = o.Substring(pos, len), rest = o.Remove(pl);//o.Substring(pl, o.Length - pl);
                string first = o.Remove(pos), c = o.Substring(pos, len), rest = o.Substring(pl, o.Length - pl);
                o = first + "<a href=\"" + eurlex_uri + "\" title=\"" + plainNQ + "\">" + c + "</a>" + rest;
#else//with_LinkHint
                string id = a.Celex,
                    eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.Language + "/TXT/?uri=CELEX:" + id;
                if (a.ReferrenceInfo != "") eurlex_uri += "#" + a.ReferrenceInfo;//"#"?
                //string first = o.Substring(pos), c = o.Substring(pos, len), rest = o.Remove(pl);//o.Substring(pl, o.Length - pl);
                string first = o.Remove(pos), c = o.Substring(pos, len), rest = o.Substring(pl, o.Length - pl);
                o = first + "<a href=\"" + eurlex_uri + "\">" + c + "</a>" + rest;
#endif//with_LinkHint
                /*
                Word.Document doc = CurrentDoc;
                Word.Range rng = doc.Range(pos, pos + len);
                string rngTxt = rng.Text,
                    eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.Language + "/TXT/?uri=CELEX:" + id;
                //if (a.ReferrenceInfo != "")eurlex_uri += "/" + a.ReferrenceInfo;
                var o = doc.Hyperlinks.Add(
                    Anchor: rng,
                    Address: eurlex_uri,
                    ScreenTip: plain,
                    SubAddress: a.ReferrenceInfo
                    //,TextToDisplay: plain
                    );
                rng.Font.ColorIndex = Word.WdColorIndex.wdBlue;//wdGreen;
                */
            }
            string f = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes;
            S2F(f, o);//S2F(@"c:\result-html.html", o);
            return true;
#else//with_SvcRef
            bool success = false;
            o = OutpHtml(html, ref success);// ProcessV3(html);
            /*20150618>>
                        o = html;
            ///20150618>>
            ///<<20150618
                        JsonResp jsonResult = this.PostRequestPlainText(o);
                        foreach (Entity a in jsonResult.results.entity)
                        {
                            int pos = a.begin,
                                len = a.len,
                                pl = pos + len;
#if with_LinkHint
                            string id = a.value,
                                doc_hint = GetDocHint(a.language, a.value, a.toPar),
                                plain = StripHtml(doc_hint),//20150616
                                plainNQ = plain.Replace("\"", ""),//20150616
                                eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.language + "/TXT/?uri=CELEX:" + id;
                            if (a.toPar != "") eurlex_uri += "#" + a.toPar;//"#"?
                            //string first = o.Substring(pos), c = o.Substring(pos, len), rest = o.Remove(pl);//o.Substring(pl, o.Length - pl);
                            string first = o.Remove(pos), c = o.Substring(pos, len), rest = o.Substring(pl, o.Length - pl);
                            o = first + "<a href=\"" + eurlex_uri + "\" title=\"" + plainNQ + "\">" + c + "</a>" + rest;
#else//with_LinkHint
                            string id = a.value,
                                eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.language + "/TXT/?uri=CELEX:" + id;
                            if (a.toPar != "") eurlex_uri += "#" + a.toPar;//"#"?
                            string first = o.Remove(pos), c = o.Substring(pos, len), rest = o.Substring(pl, o.Length - pl);
                            string clr = a.label == "reference" ? "green" : "red";
                            o = first + "<a href=\"" + eurlex_uri + "\" style=\"color:" + clr + "\">" + c + "</a>" + rest;
#endif//with_LinkHint
                        }
            <<20150618*/
            string f = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes;
            S2F(f, o);//S2F(@"c:\result-html.html", o);
            return success;
#endif//with_SvcRef
        }
#endif//!with_async
#if with_async
        public bool htmlPcsSuccess = false;
        public string htmlPcsResOut = "";
        public string htmlPcsResIn = "";
        delegate void GetProcessedHtml();
        private GetProcessedHtml tsk = null;
        private AsyncCallback cbtsk = null;
        public static void cbGetProcessedHtml(IAsyncResult obj)
        {
            if (!EUA.htmlPcsSuccess) goto l_exit;
            //string fn = System.IO.Directory.GetCurrentDirectory() + EULinksCheckerAddIn.fn_HtmlRes;// @"c:\result-html.html";
            string fn = EUA.FAFN();
            //object ofn = fn;
            S2F(fn, EUA.htmlPcsResOut);
            if (!File.Exists(fn))
            {
                EUA.htmlPcsSuccess = false;
                goto l_exit;
            }
            //EUA.WordApp.Documents.Open(ofn);
            //EUA.SaveAsDocX();//20150709
            Word.Document w=EUA.WordApp.Documents.Add();
            w.Select();
            EUA.WordApp.Selection.InsertFile(fn);
            EUA.WordApp.Selection.Start = 0;
            EUA.WordApp.Selection.End = 0;
            w.Application.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(EUA.application_WindowBeforeRightClick);
        l_exit:
            //EUA.HideBusyTM();
            EUA.Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
            EUA.SetPrintView();
            EUA.SwitchOffSpellChecker();
            string msg = EUA.htmlPcsSuccess ? Resources.Resource.cSuccess : Resources.Resource.cErrorOccured;
            //
            try
            {
                EUA.tsk.EndInvoke(obj);
                EUA.tsk = null;
                EUA.cbtsk = null;
            }
            catch { }
            //if (b) 
            System.Windows.Forms.MessageBox.Show(msg, Resources.Resource.cInformation);
        }
        public static void tskGetProcessedHtml()
        {
            EUA.ShowBusyTM();
            EUA.htmlPcsResOut = EUA.OutpHtml(EUA.htmlPcsResIn, ref EUA.htmlPcsSuccess);
            EUA.HideBusyTM();
        }
        //tsk.BeginInvoke(new AsyncCallback(cbGetProcessedHtml), tsk);
#endif//with_async
        /// <summary>
        /// Put links and terms
        /// </summary>
        /// <param name="b">clear all links from the source before setting</param>
        /// <returns>true or false depending on the success</returns>
        public bool PutLinksAndTerms(bool b)
        {
            bool success = false;
            if (CurrentDoc == null)
                return false;
#if with_LinkHint
#if with_LinkCache
            LinkCache.Clear();
#endif//with_LinkCache
            if (bUseHtml)
            {/*+*/
                Application.System.Cursor = Word.WdCursorType.wdCursorWait;
                //RemoveLinksAndTerms();
                string output = "";
                PutLinksAndTermsHtml(sHtml(), ref output);
                string fn = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes;// @"c:\result-html.html";
                object ofn = fn;
                WordApp.Documents.Open(ofn);
                WordApp.ActiveWindow.Application.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(application_WindowBeforeRightClick);
                goto l_exit;
                //Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
                //return true;
                //Word.Document ddd = WordApp.Documents.Open(ofn);
                //ddd.ActiveWindow.Application.WindowBeforeRightClick -= MyHandler;
                //ddd.ActiveWindow.Application.WindowBeforeRightClick += MyHandler;                       
                //new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(application_WindowBeforeRightClick);
            }/*+*/
            Application.System.Cursor = Word.WdCursorType.wdCursorWait;
            RemoveLinksAndTerms();
            string txt_cl = CurrentDoc.Content.Text;//Get The whole document text
            //CurrentDoc.ActiveWindow.Selection.WholeStory();
            //CurrentDoc.SelectAllEditableRanges();
            //WordApp.Selection.Text = txt_cl;
            CurrentDoc.Content.Text = txt_cl;
            ServiceReference.LinkingServiceClient cl = new ServiceReference.LinkingServiceClient();
            EUCases.EULinksCheckerWordAddIn.ServiceReference.TextPosWrapper res_cl = cl.PutLinksText(txt_cl);
            Word.Document cd = CurrentDoc;
            foreach (EUCases.EULinksCheckerWordAddIn.ServiceReference.LinkTextPos a in res_cl.LinksTextPos)
            {
                int pos = a.Pos,
                    len = a.Length;
                string id = a.Celex;
#if with_LinkHint//20150616
                string doc_hint = GetDocHint(a.Language, id, a.ReferrenceInfo),
                    plain = StripHtml(doc_hint);
#endif//with_LinkHint
                //                ShowStrInBrowser(doc_hint);
                //Word.Document cd = CurrentDoc;
                Word.Range rng = cd.Range(pos, pos + len);
                string rngTxt = rng.Text,
                    eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.Language + "/TXT/?uri=CELEX:" + id;
                //if (a.ReferrenceInfo != "")eurlex_uri += "/" + a.ReferrenceInfo;
                var o = cd.Hyperlinks.Add(
                    Anchor: rng,
                    Address: eurlex_uri,
#if with_LinkHint
                    ScreenTip: plain,
#endif//with_LinkHint
 SubAddress: a.ReferrenceInfo
                    //,TextToDisplay: plain
                    );
#if with_LinkCache
                LinkCache.Add(o, new rUrl(a, a.Language));
#endif//with_LinkCache
                rng.Font.ColorIndex = Word.WdColorIndex.wdBlue;//wdGreen;
            }
        l_exit:
            Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
            if (b) MessageBox.Show(Resources.Resource.cSuccess, Resources.Resource.cInformation);
#else //with_LinkHint
#if with_LinkCache
            LinkCache.Clear();
#endif//with_LinkCache
            if (bUseHtml)
            {/*+*/
                Application.System.Cursor = Word.WdCursorType.wdCursorWait;
                SetUtf8();
                //ShowBusyTM();
                //RemoveLinksAndTerms();
#if with_async
                try { ShowBusyTM(); }
                catch { }
                htmlPcsSuccess = false;
                htmlPcsResOut = "";
                htmlPcsResIn = sHtml();
                try { HideBusyTM(); }
                catch { }
                //GetProcessedHtml 
                tsk = new GetProcessedHtml(tskGetProcessedHtml);
                cbtsk = new AsyncCallback(cbGetProcessedHtml);
                tsk.BeginInvoke(cbtsk, tsk);
                //tsk.BeginInvoke(new AsyncCallback(cbGetProcessedHtml), tsk);
                goto l_exit1;
#else//with_async
                string output = "";
                success = PutLinksAndTermsHtml(sHtml(), ref output);
                if (!success) goto l_exit;
                string fn = System.IO.Directory.GetCurrentDirectory() + fn_HtmlRes;// @"c:\result-html.html";
                object ofn = fn;
                WordApp.Documents.Open(ofn);
                WordApp.ActiveWindow.Application.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(application_WindowBeforeRightClick);
                goto l_exit;
#endif//with_async
                //Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
                //return true;
                //Word.Document ddd = WordApp.Documents.Open(ofn);
                //ddd.ActiveWindow.Application.WindowBeforeRightClick -= MyHandler;
                //ddd.ActiveWindow.Application.WindowBeforeRightClick += MyHandler;                       
                //new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(application_WindowBeforeRightClick);
            }/*+*/
            Application.System.Cursor = Word.WdCursorType.wdCursorWait;
            RemoveLinksAndTerms();
            string txt_cl = CurrentDoc.Content.Text;//Get The whole document text
            //CurrentDoc.ActiveWindow.Selection.WholeStory();
            //CurrentDoc.SelectAllEditableRanges();
            //WordApp.Selection.Text = txt_cl;
            CurrentDoc.Content.Text = txt_cl;
            //ServiceReference.LinkingServiceClient cl = new ServiceReference.LinkingServiceClient();
            //EUCases.EULinksCheckerWordAddIn.ServiceReference.TextPosWrapper res_cl = cl.PutLinksText(txt_cl);
            Word.Document cd = CurrentDoc;
            cd.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
            JsonResp jsonResult = this.PostRequestPlainText(txt_cl);
            foreach (Entity a in jsonResult.results.entity)
            {
                int pos = a.begin,
                    len = a.len;
                string id = a.value;
#if with_LinkHint//20150616
                string doc_hint = GetDocHint(a.language, id, a.toPar),
                    plain = StripHtml(doc_hint);
#endif//with_LinkHint
                //                ShowStrInBrowser(doc_hint);
                //Word.Document cd = CurrentDoc;
                Word.Range rng = cd.Range(pos, pos + len);
                string rngTxt = rng.Text,
                    eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + a.language + "/TXT/?uri=CELEX:" + id;
                //if (a.ReferrenceInfo != "")eurlex_uri += "/" + a.ReferrenceInfo;
                var o = cd.Hyperlinks.Add(
                    Anchor: rng,
                    Address: eurlex_uri,
#if with_LinkHint
                    ScreenTip: plain,
#endif//with_LinkHint
 SubAddress: a.toPar
                    //,TextToDisplay: plain
                    );
#if with_LinkCache
                LinkCache.Add(o, new rUrl(a, a.Language));
#endif//with_LinkCache
                Word.WdColorIndex ci;
                ci = a.label == "reference" ? Word.WdColorIndex.wdGreen : Word.WdColorIndex.wdRed;
                rng.Font.ColorIndex = ci;
            }
        l_exit:
            HideBusyTM();
            Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
            SetPrintView();
            string msg = success ? Resources.Resource.cSuccess : Resources.Resource.cErrorOccured;
            if (b) MessageBox.Show(msg, Resources.Resource.cInformation);
#endif//with_LinkHint
#if with_async
        l_exit1:
#endif//with_async
            return true;
#if comm_v1
            string txt1 = CurrentDoc.Content.Text;//Get The whole document text
            MessageBox.Show(txt1);
            EuLinksResponse e = PostRequestPlainText(txt1);
#else
            #region _Old1
            //Word._Document od = Application.ActiveDocument as Word._Document;
            //object start = 0;
            //object end = 0;
            //Application.System.Cursor = Word.WdCursorType.wdCursorWait;
            //try
            //{
            //    FrontendWS.LinksFuncs lf = new FrontendWS.LinksFuncs();
            //    string theText = CurrentDoc.Content.Text;
            //    object missing = System.Reflection.Missing.Value;
            //    Word.Document doc = Application.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            //    doc.Activate();
            //    doc.Content.Text = theText;
            //    theText = doc.Content.Text;
            //    //return true;
            //    byte[] compressedArray = EucasesLinkingService.Tools.Compression.Compress(System.Text.Encoding.UTF8.GetBytes(theText));
            //    var lLinks = lf.PutLinks(compressedArray);
            //    Word.Range rng;
            //    foreach (var l in lLinks)
            //    {
            //        int pos;
            //        int len;
            //        if (l is EUCases.EULinksCheckerWordAddIn.FrontendWS.EurovocTextPos)
            //        {
            //            pos = l.Pos;
            //            len = l.Length;
            //            string id = (l as EUCases.EULinksCheckerWordAddIn.FrontendWS.EurovocTextPos).Id;
            //            rng = doc.Range(pos, pos + len);
            //            string txt = System.Web.HttpUtility.UrlEncode(rng.Text);
            //            doc.Hyperlinks.Add(
            //                Anchor: rng,
            //                Address: "http://eur-lex.europa.eu/search.html?APIS=1&instInvStatus=ALL&DTC=false&DTS_DOM=ALL&orEUROVOC=DC_DECODED%3D%22" + txt + "%22,DC_ALTLABEL%3D%22" + txt + "%22&type=advanced&lang=" + l.Language.ToLower() + "&SUBDOM_INIT=ALL_ALL&DTS_SUBDOM=ALL_ALL",
            //                ScreenTip: rng.Text
            //                );
            //            rng.Font.ColorIndex = Word.WdColorIndex.wdGreen;
            //            //WordApp.HelpTool();
            //            ////////
            //            //object txt_cmnt=rng.Text;
            //            //doc.Comments.Add(rng,ref txt_cmnt);
            //            ////////
            //        }
            //        else
            //            if (l is EUCases.EULinksCheckerWordAddIn.FrontendWS.LinkTextPos)
            //            {
            //                pos = l.Pos;
            //                len = l.Length;
            //                string celex = (l as EUCases.EULinksCheckerWordAddIn.FrontendWS.LinkTextPos).Celex;
            //                string referrenceInfo = (l as EUCases.EULinksCheckerWordAddIn.FrontendWS.LinkTextPos).ReferrenceInfo;
            //                rng = doc.Range(pos, pos + len);
            //                doc.Hyperlinks.Add(
            //                    Anchor: rng,
            //                    Address: "http://eur-lex.europa.eu/legal-content/" + l.Language + "/TXT/?APIS=1&uri=CELEX%3A" + celex + (referrenceInfo == null ? "" : "&ref=" + referrenceInfo),
            //                    ScreenTip: rng.Text
            //                    );
            //                rng.Font.ColorIndex = Word.WdColorIndex.wdBlue;
            //            }
            //    }
            //    WordApp.Selection.Start = 0;
            //    WordApp.Selection.End = 0;
            //}
            //catch (Exception exc)
            //{
            //    //MessageBox.Show(exc.InnerException.Message);
            //    MessageBox.Show(exc.Message);
            //    MessageBox.Show(exc.StackTrace);
            //}
            //finally
            //{
            //    Application.System.Cursor = Word.WdCursorType.wdCursorNormal;
            //    od.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            //}
            #endregion
#endif
            return true;
        }
        /// <summary>
        /// check if the link has been set by EuLinksChecker
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        private bool LinkIsOurs(Word.Hyperlink h)
        {
            try
            {
                if (
                    h.Address.IndexOf("http://eur-lex.europa.eu/legal-content/") < 0 &&
                    h.Address.IndexOf("http://app.eurocases.eu/api/Doc/SearchByXmlId/") < 0 &&
                    h.Range.Font.ColorIndex != Word.WdColorIndex.wdGreen &&
                    h.Range.Font.ColorIndex != Word.WdColorIndex.wdRed
                    )
                    return false;
            }
            catch { return false; }
            return true;
        }
        /// <summary>
        /// Delete certain hyperlink
        /// </summary>
        /// <param name="h">the link itself</param>
        private void Delete1Hl(Word.Hyperlink h)
        {
            if (!LinkIsOurs(h)) return;
#if with_LinkCache
            if (LinkCache.ContainsKey(h)) LinkCache.Remove(h);
#endif//with_LinkCache
            //check if we have set this link: 1)by address(contains http://eur-lex...) and 2) has color wdRed or wdGreen: todo
            try
            {
                if (h != null) if (h.Range != null) if (h.Range.Font != null) h.Range.Font.ColorIndex = Word.WdColorIndex.wdBlack;
            }
            catch { }
            //System.Windows.Forms.MessageBox.Show(hl.Range.Text.ToString() + "\n" + hl.Address.ToString());
            h.Delete();
        }
        /// <summary>
        /// Get Xml contents from the service
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string sXml(string s)
        {
            string u = "http://techno.eucases.eu/FrontEndREST/api/Links/GenerateXml/", p = "";
            try
            {
                WebRequest q = WebRequest.Create(u);
                q.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(s);
                //q.ContentType = "application/x-www-form-urlencoded";
                q.ContentLength = byteArray.Length;
                //q.Headers.Add("UI-Language", sUiLang);
                q.Timeout = 200000;
                using (var m = q.GetRequestStream())
                    m.Write(byteArray, 0, byteArray.Length);
                var r = (HttpWebResponse)q.GetResponse();
                p = new StreamReader(r.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            }
            catch (Exception e)
            {
                p = s;
            }
            return p;
        }
        /// <summary>
        /// Save to XML
        /// </summary>
        /// <returns></returns>
        public bool SaveToXml()
        {
            /*System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "XML files (*.xml)|*.xml"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                System.IO.File.WriteAllText(sfd.FileName, Properties.Settings.Default.FakeXML);*/
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "XML files (*.xml)|*.xml"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                System.IO.File.WriteAllText(sfd.FileName, sXml(sHtml()));
            return true;
        }
        /// <summary>
        /// Remove the links within the current selection
        /// </summary>
        /// <returns></returns>
        public bool RmLinkSel()
        {
            List<Word.Hyperlink> l = new List<Word.Hyperlink>();
            try
            {
                int s = Application.Selection.Start, e = Application.Selection.End;
                if (e == s) return true;
                var i = CurrentDoc.Hyperlinks.GetEnumerator();
                i.Reset();
                while (i.MoveNext())
                {
                    Word.Hyperlink h = (Word.Hyperlink)i.Current;
                    try
                    {
                        if (h.Range.Start >= s && h.Range.End <= e)
                        {
                            l.Add(h);
                            //Delete1Hl(h);
                            //Application.ScreenRefresh();
                        }
                    }
                    catch { }
                }
            }
            catch
            {
            }
            foreach (Word.Hyperlink h in l) Delete1Hl(h);
            Application.ScreenRefresh();
            return true;
        }
#if with_cite
        public class JsonCite
        {
            public int DocType { get; set; }
            public string Text { get; set; }
        }
        private string strCite(int aType) {
            Word.Hyperlink h = GetCurrentHyperlink();
            if (h == null) return "";
            rUrl r = GetRUrl(h);
            //if(r.IsTerm)return "";
            string u = "http://app.eurocases.eu/api/Doc/Cite/" + r.langid + "/" + r.celex + "/" + aType.ToString(), c = "";
            HttpWebRequest q = (HttpWebRequest)WebRequest.Create(u);
            try
            {
                q.Method = "GET";
                q.Timeout = 2000;
                q.ContentType = "text/plain";
                q.Accept = "application/json";
                HttpWebResponse p = (HttpWebResponse)q.GetResponse();
                var s = new StreamReader(p.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                c = s.ToString();
            }
            catch (Exception e){}
            return c;
        }
        private JsonCite jsonCite(int aType) {
            /*
В момента е качено на web.eucases.eu:8080

/api/Doc/Cite/{langId}/{docNumber}/{citeType}
citeType - int: ShortCite = 1, LongCite = 2

return:
             * JSON{
DocType: 1 - caselaw, 2 - legislation
Text - string
             * }
             */
            string s = strCite(aType);
            if (s == "") return null;
            return JsonConvert.DeserializeObject<JsonCite>(s);
        }
        private bool ShowTypeCite(int aType) {
#if test_cite
            JsonCite e = new JsonCite();
            e.DocType = 1;
            e.Text = aType == 1 ? "OJ L 12, 16.1.2001, p. 1–23" : "Council Regulation (EC) No 44/2001 of 22 December 2000 on jurisdiction and the recognition and enforcement of judgments in civil and commercial matters. OJ L 12, 16.1.2001, p. 1–23.";
            //e.DocType = 2;
            //e.Text = aType == 1 ? "European Court Reports 2010 I-14309, ECLI:EU:C:2010:829" : "Judgment of the Court (Fifth Chamber) of 19 September 2013. Panellinios Syndesmos Viomichanion Metapoiisis Kapnou v Ypourgos Oikonomias kai Oikonomikon and Ypourgos Agrotikis Anaptyxis kai Trofimon. Case C-373/11. European Court Reports 2013 -00000, ECLI:EU:C:2013:567";
#else//test_cite
            JsonCite e = jsonCite(aType);
            if (e == null) return false;
#endif//test_cite
            string c = aType == 1 ? Resources.Resource.cm_ShortCite : Resources.Resource.cm_LongCite,
            t=e.DocType==1?Resources.Resource.cCaseLaw:Resources.Resource.cLegislation;
            CITE.SetTxtClose(Resources.Resource.cClose);
            CITE.SetTxtCopy(Resources.Resource.cCopy);
            CITE.Show(e.Text,c,e.DocType);
            //MessageBox.Show(e.Text, Resources.Resource.cInformation + " " + c + " " + t);
            return true;
        }
        private bool ShortCite() {
            return ShowTypeCite(1);
        }
        private bool LongCite() {
            return ShowTypeCite(2);
        }
#endif//with_cite
        /// <summary>
        /// Remove the current link//i.e. the link we are on
        /// </summary>
        /// <returns></returns>
        public bool Remove1()
        {
            Word.Hyperlink h = GetCurrentHyperlink();
            if (h == null) return false;
            Delete1Hl(h);
            Application.ScreenRefresh();
            return true;
            /*Word.Hyperlink hl = null;
            var iter = Application.ActiveDocument.Hyperlinks.GetEnumerator();//CurrentDoc.Hyperlinks.GetEnumerator();
            int s = Application.Selection.Start, e = Application.Selection.End;
            iter.Reset();

            while (iter.MoveNext())
            {
                hl = (Word.Hyperlink)iter.Current;
                if (hl.Range.Start <= s && hl.Range.End >= e && hl.Address.Contains("invtest.apis.bg"))
                    break;
            }
            if (hl == null)
                return false;
            Delete1Hl(hl);
            Application.ScreenRefresh();
            return true;*/
        }
        /// <summary>
        /// Simple arithmetic fiunction to show if two pairs have something in common //check for intersection
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private static bool DoCover(int x1, int x2, int y1, int y2)
        {
            return (x1 >= y1 && x1 <= y2) || (x2 >= y1 && x2 <= y2) || (y1 >= x1 && y1 <= x2) || (y2 >= x1 && y2 <= x2);
        }
        public bool CannotAddLink()
        {
            int s = Application.Selection.Start, e = Application.Selection.End;
            if (s == e)
                return true;
            var i = CurrentDoc.Hyperlinks.GetEnumerator();
            i.Reset();
            while (i.MoveNext())
            {
                var hl = (Word.Hyperlink)i.Current;
                try
                {
                    if (DoCover(hl.Range.Start, hl.Range.End, s, e)) return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// a function to add a new link
        /// </summary>
        /// <returns>the success of the operation</returns>
        public bool AddNewLink()
        {
            if (CannotAddLink()) return false;
            string uri = "";
            if (InputBox.Show(Resources.Resource.cLinkTo, Resources.Resource.cPlzEnterCompleteUrl, ref uri) == System.Windows.Forms.DialogResult.OK)
            {
                CurrentDoc.Hyperlinks.Add(
                    Anchor: WordApp.Selection.Range,
                    Address: uri//"http://localhost:100000",//cu_Link,
                    //SubAddress: "Manual"
                    );
            }
            Application.ScreenRefresh();
            return true;
        }
        /// <summary>
        /// GetCurrentHyperLink
        /// </summary>
        /// <returns>the hyperlink we are on</returns>
        public Word.Hyperlink GetCurrentHyperlink()
        {
            try
            {
                var i = CurrentDoc.Hyperlinks.GetEnumerator();
                int s = Application.Selection.Start, e = Application.Selection.End;
                i.Reset();
                while (i.MoveNext())
                {
                    Word.Hyperlink h = (Word.Hyperlink)i.Current;
                    try
                    {
                        if (h.Range.Start <= s && h.Range.End >= e) return h;
                    }
                    catch { }
                }
            }
            catch
            {
            }
            return null;
        }
        /// <summary>
        /// Open the form fCredentials currently used as "settings" form
        /// </summary>
        /// <returns></returns>
        public bool EditCredentials()
        {
            EUCases.EULinksCheckerWordAddIn.Forms.fCredentials f = new Forms.fCredentials();
            f.ShowDialog();
            return true;
        }
        #region Browser stuff
        /*
                private void DisplayHtml(string html)
                {
        Instead of navigating to blank, you can do:
        webBrowser1.DocumentText="0";
        webBrowser1.Document.OpenNew(true);
        webBrowser1.Document.Write(theHTML);
        webBrowser1.Refresh();
                    webBrowser1.Navigate("about:blank");
                    if (webBrowser1.Document != null)
                    {
                        webBrowser1.Document.Write(string.Empty);
                    }
                    webBrowser1.DocumentText = html;
                }
        */
        /// <summary>
        /// String to File
        /// </summary>
        /// <param name="f">the file name</param>
        /// <param name="s">the contents to be saved to the file of name f</param>
        private static void S2F(string f, string s)
        {
            if (System.IO.File.Exists(f))
                System.IO.File.Delete(f);
            System.IO.File.WriteAllText(f, s);
        }
        /// <summary>
        /// File to String
        /// </summary>
        /// <param name="f">the file name</param>
        /// <returns>the contents of the file as string</returns>
        private static string F2S(string f)
        {
            string r = "";
            if (!System.IO.File.Exists(f)) goto e;
            //r = System.IO.File.ReadAllText(f);
            using (FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader tr = new StreamReader(fs))
            {
                r = tr.ReadToEnd();
            }
        e:
            return r;
        }
        /// <summary>
        /// Get the path of the dafault browser
        /// </summary>
        /// <returns></returns>
        private static string GetDefaultBrowserPath()
        {
            string key = @"HTTP\shell\open\command";
            using (RegistryKey registrykey = Registry.ClassesRoot.OpenSubKey(key, false))
                return ((string)registrykey.GetValue(null, null)).Split('"')[1];
        }
        // creates a process and passes the url as an argument to the process
        /// <summary>
        /// Open the default browser and navigate to a certain address
        /// </summary>
        /// <param name="url">the address to navigate to</param>
        private static void Navigate(string url)
        {
            Process p = new Process();
            p.StartInfo.FileName = GetDefaultBrowserPath();
            p.StartInfo.Arguments = url;
            p.Start();
        }
        /// <summary>
        /// Show string in the default browser
        /// </summary>
        /// <param name="s">the html content to be visualized</param>
        private static void ShowStrInBrowser(string s)
        {
            string f = System.IO.Directory.GetCurrentDirectory() + fn_TmpHtmlFromResp;
            S2F(f, s);
            Navigate(f);
            System.Threading.Thread.Sleep(1000);
            System.IO.File.Delete(f);
        }
        #endregion
#if comm_v1
        #region Summary
        public class Summary
        {
            public string coveredText { get; set; }
            public string end { get; set; }
            public string begin { get; set; }
        }

        public class Descriptor
        {
            public string uid { get; set; }
            public string value { get; set; }
            public string coveredText { get; set; }
            public string confidence { get; set; }
            public string end { get; set; }
            public string begin { get; set; }
        }

        public class Entity
        {
            public string value { get; set; }
            public string coveredText { get; set; }
            public string label { get; set; }
            public string end { get; set; }
            public string begin { get; set; }
        }

        public class Results
        {
            public List<Summary> summary { get; set; }
            public List<Descriptor> descriptor { get; set; }
            public List<Entity> entity { get; set; }
        }

        public class EuLinksResponse
        {
            public string status { get; set; }
            public Results results { get; set; }
        }
        #endregion
        #region Communication
        private EuLinksResponse PostRequestPlainText(String s)
        {
            EuLinksResponse e = null;
            var request = (HttpWebRequest)WebRequest.Create("http://techno.eucases.eu:8080/nlp-toolkit/rest/textminingservice/analyse");
            var postData = s;
            request.Method = "POST";
            request.Timeout = 2000000;
            request.ContentType = "text/plain";
            request.Accept = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var res = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            e = JsonConvert.DeserializeObject<EuLinksResponse>(res.ToString());
            return e;
        }
        #endregion
#if hint
http://app.eurocases.eu/api/Doc/ParHint/{LangId}/{docNumber}/{toPar}
Връща JSON с поле HtmlContent. В момента като не е открито подаденото нещо в нашата база връща временнен текст, 
който ще сменя с линк към документа в eur-lex.eu

Пример:
http://app.eurocases.eu/api/Doc/ParHint/1/32009r1122/art2_al23
public class HintObject
{
    public string HtmlContent { get; set; }
}
#endif
        /*
string txt = Doc.Content.Text;//Get The whole document text
EuLinksResponse e = PostRequestPlainText(txt);
int i;
Word.Range rr;
for (i = 0; i < e.results.summary.Count; i++)
{
    rr = Doc.Range(e.results.summary[i].begin, e.results.summary[i].end);
    Doc.Comments.Add(rr, e.results.summary[i].coveredText);
}
for (i = 0; i < e.results.descriptor.Count; i++)
{
    rr = Doc.Range(e.results.descriptor[i].begin, e.results.descriptor[i].end);
    Doc.Comments.Add(rr, e.results.descriptor[i].coveredText);
}
*/
#endif
#if comm_v2
        /// <summary>
        /// The following class is designed to handle synchronous processing
        /// </summary>
        public class Entity
        {
            public string value { get; set; }
            public string coveredText { get; set; }
            public string label { get; set; } //ref|term
            public int end { get; set; }
            public int begin { get; set; }
            public string toPar { get; set; }
            public string language { get; set; }
            public int len { get { return end - begin; } }
            /*internal Entity Copy()
            {
                var newEnt = new Entity();
                newEnt.begin = this.begin;
                newEnt.end = this.end;
                newEnt.label = this.label;
                newEnt.coveredText = this.coveredText;
                newEnt.value = this.value;
                return newEnt;
            }*/
        }
        public enum EntityType
        {
            StartReference,
            EndReference,
            StartConcept,
            EndConcept,
            StartTerm,
            EndTerm,
        }
        public class EntityWrapper : IComparable
        {
            public Entity Entity { get; set; }
            public string EntityNode { get; set; }
            public EntityType EntityType { get; set; }
            public int CompareTo(object obj)
            {
                var entObj = obj as EntityWrapper;
                if (entObj != null)
                {
                    if (this.EntityType == EntityType.EndReference && entObj.EntityType == EntityType.EndConcept)
                        return 1;
                    else if (this.EntityType == EntityType.EndConcept && this.EntityType == EntityType.EndReference)
                        return -1;

                    else if (this.EntityType == EntityType.StartReference && entObj.EntityType == EntityType.StartConcept)
                        return -1;
                    else if (this.EntityType == EntityType.StartConcept && entObj.EntityType == EntityType.StartReference)
                        return 1;
                    else
                        return 0;
                }
                return 0;
            }
        }        /*public class EntityWrapper : IComparable
        {
            public Entity Entity { get; set; }
            public string EntityNode { get; set; }
            public EntityType EntityType { get; set; }
            public int CompareTo(object obj)
            {
                var entObj = obj as EntityWrapper;
                if (entObj != null)
                {
                    if (this.EntityType == EntityType.EndReference && entObj.EntityType == EntityType.EndConcept)
                        return 1;
                    else if (this.EntityType == EntityType.EndConcept && this.EntityType == EntityType.EndReference)
                        return -1;
                    else if (this.EntityType == EntityType.StartReference && entObj.EntityType == EntityType.StartConcept)
                        return -1;
                    else if (this.EntityType == EntityType.StartConcept && entObj.EntityType == EntityType.StartReference)
                        return 1;
                    else
                        return 0;
                }
                return 0;
            }
        }*/
        public class JsonResp
        {
            public string status { get; set; }
            public Results results { get; set; }
        }
        public class Results
        {
            public List<Entity> entity { get; set; }
            public Results()
            {
                this.entity = new List<Entity>();
            }
        }
        private JsonResp PostRequestPlainText(String s)
        {
            JsonResp e = null;
            //http://techno.eucases.eu/FrontEndREST/api/Links/PutHtmlLinks/
            string surl = bUseHtml ? "http://techno.eucases.eu/FrontEndREST/api/Links/PutLinksHtmlPipeline" : "http://techno.eucases.eu/FrontEndREST/api/Links/PutLinksTextPipeline";
            // Alternative : http://techno.eucases.eu/FrontEndREST/api/Links/PutLinksTextPipeline (plain text)
            var request = (HttpWebRequest)WebRequest.Create(surl);
            var postData = s;
            request.Method = "POST";
            request.Timeout = 2000000;
            request.ContentType = "text/plain";
            request.Accept = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(byteArray, 0, byteArray.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var res = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            e = JsonConvert.DeserializeObject<JsonResp>(res.ToString());
            var tmp = e.results.entity.ToList();
            e.results.entity.Clear();
            e.results.entity.AddRange(tmp.OrderByDescending(x => x.begin));
            //e.results.entity.Reverse();
            return e;
        }
        /// <summary>
        /// Get response from a service designed to "append" EuLinks links
        /// </summary>
        /// <param name="InpHtml">The Html Source of the document</param>
        /// <param name="success">result of the operation</param>
        /// <returns>the processed html source</returns>
        public string OutpHtml(string InpHtml, ref bool success)//MainMethod Variant 2
        {
            string responseFromServer = "";
            try
            {
                string u = "http://techno.eucases.eu/FrontEndREST/api/Links/PutHtmlLinksWordAddin/";
                //string u = "http://techno.eucases.eu/FrontEndREST/api/Links/PutHtmlLinks/";//
                WebRequest request = WebRequest.Create(u);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(InpHtml);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Headers.Add("UI-Language", sUiLang);
                request.Timeout = 200000;
                using (var stream = request.GetRequestStream())
                    stream.Write(byteArray, 0, byteArray.Length);
                var response = (HttpWebResponse)request.GetResponse();
                responseFromServer = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                success = true;
            }
            catch (Exception e)
            {
                success = false;
                responseFromServer = InpHtml;
            }
            return responseFromServer;
        }
        #region Maybe later
        public string ProcessV1(string html, JsonResp jsonResp)//html is InpHtml; MainMethod Variant 1
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            if (jsonResp != null && jsonResp.status == "OK")
            {
                if (jsonResp.results.entity != null && jsonResp.results.entity.Count > 0)
                {
                    // string hint = AddHint(jsonResp.results.entity, langId, uiLangId);
                    // sb.Append(hint);
                    Dictionary<int, List<EntityWrapper>> dict = this.PopulateEntityDictionary(jsonResp);//, uiLang);
                    foreach (var item in dict)
                    {
                        sb.Append(html.Substring(index, item.Key - index));
                        index = item.Key;
                        foreach (var node in item.Value)
                        {
                            sb.Append(node.EntityNode);
                        }
                    }
                }
            }
            sb.Append(html.Substring(index, html.Length - index));
            return sb.ToString();
        }
        private Dictionary<int, List<EntityWrapper>> PopulateEntityDictionary(JsonResp jsonResp)//, string uiLang)
        {
            List<Entity> entities = jsonResp.results.entity.Where(x => x.label == "reference" || x.label == "concept" || x.label == "term").OrderBy(x => x.begin).ToList();
            Dictionary<int, List<EntityWrapper>> dict = new Dictionary<int, List<EntityWrapper>>();
            foreach (var entity in entities)
            {
                var newEntityWrapperStart = new EntityWrapper();
                newEntityWrapperStart.Entity = entity;
                var newEntityWrapperEnd = new EntityWrapper();
                newEntityWrapperEnd.Entity = entity;
                if (entity.label == "reference")
                {
                    newEntityWrapperStart.EntityType = EntityType.StartReference;
                    string id = entity.value,
                        eurlex_uri = "http://eur-lex.europa.eu/legal-content/" + entity.language + "/TXT/?uri=CELEX:" + id,
                        //clr = entity.label == "reference" ? "green" : "red";
                        clr = "green";
                    if (entity.toPar != "") eurlex_uri += "#" + entity.toPar;//"#"?
                    newEntityWrapperStart.EntityNode = "<a href=\"" + eurlex_uri + "\" style=\"color:" + clr + "\">";
                    if (dict.ContainsKey(entity.begin))
                    {
                        dict[entity.begin].Add(newEntityWrapperStart);
                        dict[entity.begin].Sort();
                    }
                    else
                    {
                        dict.Add(entity.begin, new List<EntityWrapper>() { newEntityWrapperStart });
                    }
                    newEntityWrapperEnd.EntityType = EntityType.EndReference;
                    newEntityWrapperEnd.EntityNode = "</a>";
                    if (dict.ContainsKey(entity.end))
                    {
                        dict[entity.end].Add(newEntityWrapperEnd);
                        dict[entity.end].Sort();
                    }
                    else
                    {
                        dict.Add(entity.end, new List<EntityWrapper>() { newEntityWrapperEnd });
                    }
                }
                else if (entity.label == "concept" || entity.label == "term")
                {
                    int lastKey = 0;
                    if (dict.Keys.Count > 0)
                    {
                        lastKey = dict.Keys.Max();
                    }
                    if (entity.begin < lastKey && entity.end > lastKey)
                    {
                        int end = entity.end;
                        entity.end = lastKey;
                        this.AddConcept(newEntityWrapperStart, newEntityWrapperEnd, entity, dict);

                        entity.begin = lastKey;
                        entity.end = end;
                        this.AddConcept(newEntityWrapperStart, newEntityWrapperEnd, entity, dict);
                    }
                    else
                    {
                        this.AddConcept(newEntityWrapperStart, newEntityWrapperEnd, entity, dict);
                    }
                }
            }
            return dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        private void AddConcept(EntityWrapper newEntityWrapperStart, EntityWrapper newEntityWrapperEnd, Entity entity, Dictionary<int, List<EntityWrapper>> dict)
        {
            newEntityWrapperStart.EntityType = EntityType.StartConcept;
            //newEntityWrapperStart.EntityNode = string.Format("<span class='concept' data-id='{0}'>", entity.value);
            newEntityWrapperStart.EntityNode = "<span style=\"color:red\">";
            if (dict.ContainsKey(entity.begin))
            {
                dict[entity.begin].Add(newEntityWrapperStart);
                dict[entity.begin].Sort();
            }
            else
            {
                dict.Add(entity.begin, new List<EntityWrapper>() { newEntityWrapperStart });
            }
            newEntityWrapperEnd.EntityType = EntityType.EndConcept;
            newEntityWrapperEnd.EntityNode = "</span>";
            if (dict.ContainsKey(entity.end))
            {
                dict[entity.end].Add(newEntityWrapperEnd);
                dict[entity.end].Sort();
            }
            else
            {
                dict.Add(entity.end, new List<EntityWrapper>() { newEntityWrapperEnd });
            }
        }
        #endregion
#endif
    }
}