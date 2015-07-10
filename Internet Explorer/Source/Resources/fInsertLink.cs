using EUCases.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EUCases.Resources
{
    public partial class fInsertLink : Form
    {
        public string Hyperlink { get; set; }
        public fInsertLink()
        {
            InitializeComponent();

            string lang;
            RegistryHelper.GetLanguage(out lang);
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }

            this.Text = LanguagesHelper.Text("cInsertLink", lang);
            this.lblInsertLink.Text = LanguagesHelper.Text("cPlzEnterCompleteUrl", lang);
            this.bOK.Text = LanguagesHelper.Text("cOK", lang);
            this.bCancel.Text = LanguagesHelper.Text("cCancel", lang);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hyperlink = this.tbHyperlink.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
