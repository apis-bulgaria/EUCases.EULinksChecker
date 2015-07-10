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
    public partial class fMessageBox : Form
    {
        public fMessageBox()
        {
            InitializeComponent();
        }

        public fMessageBox(string message)
        {
            InitializeComponent();
            string lang;
            RegistryHelper.GetLanguage(out lang);
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }

            this.Text = LanguagesHelper.Text("cConfirmation", lang);
            this.tbMessageText.Text = message;
            this.bOK.Text = LanguagesHelper.Text("cOK", lang);
            this.bCancel.Text = LanguagesHelper.Text("cCancel", lang);
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
