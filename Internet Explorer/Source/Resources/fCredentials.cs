using EUCases.Classes;
using EUCases.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EUCases.Forms
{
    public partial class fCredentials : Form
    {
        public fCredentials()
        {
            InitializeComponent();
            UserRegistryData ud = new UserRegistryData()
            {
                Language = "en"
            };
            Init(ud);
        }

        public fCredentials(UserRegistryData data)
        {
            InitializeComponent();
            Init(data);
        }

        private void Init(UserRegistryData data)
        {
            string lang;
            RegistryHelper.GetLanguage(out lang);
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }

            this.Text = LanguagesHelper.Text("cStCredentials", lang);

            this.lblInterfaceLang.Text = LanguagesHelper.Text("cLang", lang);
            this.bOK.Text = LanguagesHelper.Text("cOK", lang);
            this.bCancel.Text = LanguagesHelper.Text("cCancel", lang);

            this.tbUsername.Text = data.UserName;
            this.tbPassword.Text = data.Password;

            this.cbLang.Items.AddRange(new object[] {
                "Български [bg]",
                "English [en]",
                "Deutsch [de]",            
                "French [fr]",
                "Italiano [it]",
            });

            if (string.IsNullOrEmpty(data.Language))
            {
                this.cbLang.SelectedIndex = LangIndxAbbr["en"];
            }

            this.cbLang.SelectedIndex = LangIndxAbbr[data.Language];
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void UserName_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void bOK_Click(object sender, EventArgs e)
        {
            string lang;
            RegistryHelper.GetLanguage(out lang);
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }
            fMessageBox mb = new fMessageBox(Environment.NewLine + LanguagesHelper.Text("cMsgChgLng", lang));
            mb.ShowDialog();

            if (mb.DialogResult == DialogResult.OK)
            {
                // Set values to the registry            
                string username = "eucases";//this.tbUsername.Text;
                string password = "password";//this.tbPassword.Text;
                lang = this.SelectedLanguageAbbr;

                RegistryHelper.SetValues(username, password, lang);

                if (OnLangChanged != null)
                {
                    OnLangChanged(lang);
                }

                this.Close();
            }
        }

        public delegate void LanguageChanedDelegate(string lang);

        public LanguageChanedDelegate OnLangChanged { get; set; }

        private string SelectedLanguageAbbr
        {
            get
            {
                switch (this.cbLang.SelectedIndex)
                {
                    case 0:
                        return "bg";
                        break;
                    case 1:
                        return "en";
                        break;
                    case 2:
                        return "de";
                        break;
                    case 3:
                        return "fr";
                        break;
                    case 4:
                        return "it";
                        break;
                }

                return "en";
            }
        }

        private Dictionary<string, int> LangIndxAbbr = new Dictionary<string, int>
        {
            {"bg", 0},
            {"en", 1},
            {"de", 2},
            {"fr", 3},
            {"it", 4}
        };
    }
}
