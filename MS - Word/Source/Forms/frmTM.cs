using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace EUCases.EULinksCheckerWordAddIn.Forms
{
    public partial class frmTM : Form
    {
        public frmTM()
        {
            InitializeComponent();
        }
        public void SetMsg(string s)
        {
            this.label1.Text = s;
        }
        /*[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages. 
            switch (m.Msg)
            {
                // The WM_ACTIVATEAPP message occurs when the application 
                // becomes the active application or becomes inactive. 
                case WM_ACTIVATEAPP:
                    appActive = (((int)m.WParam != 0));
                    this.Invalidate();
                    break;
            }
            base.WndProc(ref m);
        }*/
     }
}
