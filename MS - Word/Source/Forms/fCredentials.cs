using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace EUCases.EULinksCheckerWordAddIn.Forms
{
    public partial class fCredentials : Form
    {
        private bool bEnableCbChg = false;
        /// <summary>
        /// private member to store the ItemIndex of the Interface language and serve as a basis to detect changes
        /// </summary>
        private int _SelIdx = 0;
        /// <summary>
        /// constructor of the form
        /// </summary>
        public fCredentials()
        {
            EULinksCheckerAddIn.InitRes();
            InitializeComponent();
            ApplLng();
        }
        private void SetSelIdx()
        {
            int i = 1;
            switch (EULinksCheckerAddIn.LCID)
            {
                case 7: i = 2; break;//de
                case 9: i = 1; break;//en
                case 12: i = 3; break;//fr
                case 16: i = 4; break;//it
                case 2:
                case 1026: i = 0; break;//bg
            }
            this.cbLang.SelectedIndex = i;
            _SelIdx = i;
        }
        private void ApplLng()
        {
            /*
             * useHtmlLinksProcessor
             * RemoveLinksBeforeSetLinks
             * ShowSuccessMsg: bools
             */
            Text = Resources.Resource.cm_Credentials;
            label2.Text = Resources.Resource.cPassword;
            bOK.Text = Resources.Resource.cOK;
            bCancel.Text = Resources.Resource.cCancel;
            //UserName.Text = Resources.Resource.cUserName;
            lLang.Text = Resources.Resource.cLang;
            //textBox1.Text = EULinksCheckerAddIn.RdUser();
            SetSelIdx();
        }
        private void bOK_Click(object sender, EventArgs e)
        {//20150709
            int i = cbLang.SelectedIndex;
            if (_SelIdx != i)
            {
                _SelIdx = i;
                EULinksCheckerAddIn.SetRes(i);
                ApplLng();
            }
            //EULinksCheckerAddIn.WrUser(textBox1.Text);
            Close();
        }
        private void bCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;//20150709
            if (!bEnableCbChg) return;
            EULinksCheckerAddIn.SetRes(cbLang.SelectedIndex);
            ApplLng();
        }
        private void fCredentials_Load(object sender, EventArgs e)
        {
            lLang.Top += 20;
            cbLang.Top += 20;
            SetSelIdx();
            bEnableCbChg = true;
        }
    }
}
