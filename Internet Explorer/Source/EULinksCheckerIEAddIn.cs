using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BandObjectsLib;
using SHDocVw;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net;
using System.Security.Permissions;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using EUCases.Forms;
using System.Text;
using EUCases.Classes;
using System.Globalization;
using System.Collections.Generic;
using EUCases.Resources;

namespace EUCases
{
    [Guid(Constants.ToolbarGUID)]
    [BandObject(Constants.ToolbarName, BandObjectStyle.Horizontal | BandObjectStyle.ExplorerToolbar, HelpText = Constants.ToolbarHelpText)]
    public partial class EULinksCheckerIEAddIn : BandObject
    {
        public EULinksCheckerIEAddIn()
        {
            ExplorerAttached += new EventHandler(Toolbar_ExplorerAttached);
            InitializeComponent();
            try
            {
                this.SetButtonsLang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref DESKBANDINFO dbi)
        {
            this.SetButtonsLang();

            dbi.ptMinSize.X = this.toolStrip.Width;
            dbi.ptIntegral.X = 5;

            dbi.ptMinSize.Y = this.toolStrip.Height;
            dbi.ptIntegral.Y = 5;

            //if ((dbi.dwMask & DBIM.TITLE) != 0)
            //{
            //    dbi.wszTitle = this.Title;
            //}
            dbi.dwMask &= ~DBIM.BKCOLOR;

            // Set this flags to best feet to your desired toolbar layout behaviour
            dbi.dwModeFlags = DBIMF.USECHEVRON | DBIMF.UNDELETEABLE | DBIMF.FIXED;// | DBIMF.VARIABLEHEIGHT | DBIMF.ALWAYSGRIPPER;
        }

        private SHDocVw.WebBrowser Browser
        {
            get
            {
                return (Explorer.Parent as SHDocVw.WebBrowser);
            }
        }

        private mshtml.IHTMLDocument2 HtmlDocument
        {
            get
            {
                return ((Explorer as SHDocVw.WebBrowserClass).Document as mshtml.IHTMLDocument2);
            }
        }

        private void AttachScripts()
        {
            if (!HtmlDocument.body.outerHTML.Contains("#eucases"))
            {
                string script = EUCases.Properties.Resources.Script;//this.resources.GetString("Script");
                HtmlDocument.parentWindow.execScript(script);
                string style = "<style>" + EUCases.Properties.Resources.Style + "</style>";//this.resources.GetString("Style");
                HtmlDocument.body.innerHTML += style;
                string lang;
                RegistryHelper.GetLanguage(out lang);
                if (string.IsNullOrEmpty(lang))
                {
                    lang = "en";
                }
                HtmlDocument.parentWindow.execScript("EUCASES_UI_LANG = '" + lang + "';");
            }
        }

        private void SetButtonsLang()
        {
            UserRegistryData ud;

            if (RegistryHelper.GetUserSettings(out ud))
            {
                this.InitButtonsByLanguage(ud.Language);
            }
            else
            {
                this.InitButtonsByLanguage("en");
            }
        }

        private System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EULinksCheckerIEAddIn));

        void Toolbar_ExplorerAttached(object sender, EventArgs e)
        {
            // Code here what you like to do after the Explorer is attached to the toolbar            
        }

        private string Lang
        {
            get
            {
                string lang;
                RegistryHelper.GetLanguage(out lang);
                if (string.IsNullOrEmpty(lang))
                {
                    lang = "en";
                }

                return lang;
            }
        }

        #region Buttons functionality
        /// <summary>
        /// Set links to the current page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProcessText_Click(object sender, System.EventArgs e)
        {
            try
            {
                AttachScripts();
                fMessageBox mb = new fMessageBox(LanguagesHelper.Text("cPlzConfirm", Lang));
                mb.ShowDialog();
                if (mb.DialogResult == DialogResult.OK)
                {
                    HtmlDocument.parentWindow.execScript("eucasesSetLinks();");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Remove all links from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveAllLinks_Click(object sender, System.EventArgs e)
        {
            try
            {
                AttachScripts();

                if (!(HtmlDocument.body.innerHTML.Contains("class=\"eucases-term\"") || HtmlDocument.body.innerHTML.Contains("class=\"eucases-link\"")))
                {
                    MessageBox.Show(LanguagesHelper.Text("cMsgNoLinksSetByELCFound", Lang));
                }
                else
                {
                    fMessageBox mb = new fMessageBox(Environment.NewLine + LanguagesHelper.Text("cPlzConfirmRmLnk", Lang));
                    mb.ShowDialog();
                    if (mb.DialogResult == DialogResult.OK)
                    {
                        HtmlDocument.parentWindow.execScript("eucasesRemoveAllLinks();");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Add new link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewLink_Click(object sender, System.EventArgs e)
        {
            try
            {
                AttachScripts();

                var iRange = this.HtmlDocument.selection.createRange() as mshtml.IHTMLTxtRange;
                string temp = iRange.htmlText;

                if (string.IsNullOrEmpty(temp))
                {
                    MessageBox.Show(LanguagesHelper.Text("cCannotAddLink", Lang));
                }
                else
                {
                    fInsertLink ilf = new fInsertLink();
                    ilf.ShowDialog();

                    if (ilf.DialogResult == DialogResult.OK && !string.IsNullOrEmpty(ilf.Hyperlink))
                    {
                        iRange.pasteHTML("<a href='" + ilf.Hyperlink + "' target='_blank'>" + iRange.htmlText.Replace("\r\n", "") + "</a>");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Remove selected link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveCurrentLink_Click(object sender, System.EventArgs e)
        {
            try
            {
                AttachScripts();

                var iRange = this.HtmlDocument.selection.createRange() as mshtml.IHTMLTxtRange;

                if (!string.IsNullOrEmpty(iRange.htmlText))
                {
                    fMessageBox mb = new fMessageBox(Environment.NewLine + LanguagesHelper.Text("cPlzConfirmRmLnkSel", Lang));
                    mb.ShowDialog();
                    if (mb.DialogResult == DialogResult.OK)
                    {
                        HtmlDocument.parentWindow.execScript("eucasesRemoveCurrentLink();");
                    }
                }
                else
                {
                    MessageBox.Show(LanguagesHelper.Text("cPlsSelLink", Lang));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Save document to XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveToXML_Click(object sender, System.EventArgs e)
        {
            try
            {
                AttachScripts();
                HtmlDocument.parentWindow.execScript("eucasesDownloadXml();");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Credentials button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCredentials_Click(object sender, System.EventArgs e)
        {
            UserRegistryData ud;
            bool isRegistered = RegistryHelper.GetUserSettings(out ud);

            fCredentials f = null;

            if (isRegistered)
            {
                f = new fCredentials(ud);
            }
            else
            {
                f = new fCredentials();
            }

            f.OnLangChanged += this.ChangeLang;
            f.ShowDialog();
        }
        #endregion

        private void ChangeLang(string lang)
        {
            HtmlDocument.parentWindow.execScript("EUCASES_UI_LANG = '" + lang + "';");
        }

        private void InitButtonsByLanguage(string langAbbr)
        {
            this.btnProcessText.Text = LanguagesHelper.Text("cCheckFrLinks", langAbbr);
            this.btnProcessText.ToolTipText = LanguagesHelper.Text("cStPutLinksAndTerms", langAbbr);
            this.btnRemoveLink.Text = LanguagesHelper.Text("cRemoveLinks", langAbbr);
            this.btnRemoveLink.ToolTipText = LanguagesHelper.Text("cStRemoveLinksAndTerms", langAbbr);
            this.btnAddNewLink.Text = LanguagesHelper.Text("cInsertLink", langAbbr);
            this.btnAddNewLink.ToolTipText = LanguagesHelper.Text("cStAddNewLink", langAbbr);
            this.btnRemoveCurrentLink.Text = LanguagesHelper.Text("cRemoveLinksFromSelection", langAbbr);
            this.btnRemoveCurrentLink.ToolTipText = LanguagesHelper.Text("cStRemove1", langAbbr);
            this.btnSaveToXML.Text = LanguagesHelper.Text("Save2Xml", langAbbr);
            this.btnSaveToXML.ToolTipText = LanguagesHelper.Text("cStSave2Xml", langAbbr);
            this.btnSettings.Text = LanguagesHelper.Text("cStCredentials", langAbbr);
            this.btnSettings.ToolTipText = LanguagesHelper.Text("cStCredentials", langAbbr);
        }
    }
}