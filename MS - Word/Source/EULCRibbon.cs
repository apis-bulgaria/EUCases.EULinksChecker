using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Word;
namespace EUCases.EULinksCheckerWordAddIn
{
    public partial class EULCRibbon
    {
        /// <summary>
        /// Load event handler; ensures localization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EULCRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            ApplLng(this);
        }
        /// <summary>
        /// Apply interfacxe language
        /// </summary>
        /// <param name="x">the target object of type EULCRibbon</param>
        public static void ApplLng(EULCRibbon x)
        {
            EULinksCheckerAddIn.TheRibbon = x;
            EULinksCheckerAddIn.InitRes();
            x.bAddNewLink.Label = Resources.Resource.cInsertLink;
            x.bAddNewLink.ScreenTip = Resources.Resource.cStAddNewLink;
            x.bRemove1.Label = Resources.Resource.cRemoveLinksFromSelection;
            x.bRemove1.ScreenTip = Resources.Resource.cStRemove1;
            x.bRemoveLinksAndTerms.Label = Resources.Resource.cRemoveLinks;
            x.bRemoveLinksAndTerms.ScreenTip = Resources.Resource.cStRemoveLinksAndTerms;
            x.bPutLinksAndTerms.Label = Resources.Resource.cCheckFrLinks;
            x.bPutLinksAndTerms.ScreenTip = Resources.Resource.cStPutLinksAndTerms;
            x.group1.Label = Resources.Resource.cOperations;
            x.bCredentials.Label = Resources.Resource.cm_Credentials;
            x.bCredentials.ScreenTip = Resources.Resource.cStCredentials;
            x.bSaveToXml.Label = Resources.Resource.Save2Xml;
            x.bSaveToXml.ScreenTip = Resources.Resource.cStSave2Xml;
            x.tEULC.Label = Resources.Resource.cEULinksChecker;
        }
        /// <summary>
        /// Event handler for the PutLinksAndTerms button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPutLinksAndTerms_Click(object sender, RibbonControlEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show(Resources.Resource.cPlzConfirm, Resources.Resource.cConfirmation, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (!EULinksCheckerAddIn.bUseHtml)
                    Globals.EULinksCheckerAddIn.PutLinksAndTerms(false);
                Globals.EULinksCheckerAddIn.PutLinksAndTerms(true);//20150518
                //bPutLinksAndTerms.Enabled = false;
                //bRemoveLinksAndTerms.Enabled = true;
            }
        }
        /// <summary>
        /// Event handler for the RemoveLinksAndTerms button
        /// </summary>
        private void bRemoveLinksAndTerms_Click(object sender, RibbonControlEventArgs e)
        {
            if (!Globals.EULinksCheckerAddIn.CanRemoveLinksAndTerms()) System.Windows.Forms.MessageBox.Show(Resources.Resource.cMsgNoLinksSetByELCFound, Resources.Resource.cInformation, System.Windows.Forms.MessageBoxButtons.OK);
            else if (System.Windows.Forms.MessageBox.Show(Resources.Resource.cPlzConfirmRmLnk, Resources.Resource.cConfirmation, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.EULinksCheckerAddIn.RemoveLinksAndTerms();
                //bPutLinksAndTerms.Enabled = true;
                //bRemoveLinksAndTerms.Enabled = false;
            }
        }
        /// <summary>
        /// Event handler for the SaveToXml button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bSaveToXml_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.EULinksCheckerAddIn.SaveToXml();
        }
        /// <summary>
        /// Event handler for the Remove1 button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRemove1_Click(object sender, RibbonControlEventArgs e)
        {
            bool b = Globals.EULinksCheckerAddIn.HasLinkInSelection();
            string s = b ? Resources.Resource.cPlzConfirmRmLnkSel : Resources.Resource.cPlsSelLink;
            System.Windows.Forms.MessageBoxButtons t = b ? System.Windows.Forms.MessageBoxButtons.YesNo : System.Windows.Forms.MessageBoxButtons.OK;
            if (b)
            {
                if (System.Windows.Forms.MessageBox.Show(s, Resources.Resource.cConfirmation, t) == System.Windows.Forms.DialogResult.Yes)
                    Globals.EULinksCheckerAddIn.RmLinkSel();
            }
            else System.Windows.Forms.MessageBox.Show(s, Resources.Resource.cInformation, t);
        }
        /// <summary>
        /// Event handler for the AddNewLink button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bAddNewLink_Click(object sender, RibbonControlEventArgs e)
        {
            if (Globals.EULinksCheckerAddIn.CannotAddLink())
            {
                System.Windows.Forms.MessageBox.Show(Resources.Resource.cCannotAddLink);
                return;
            }
            Globals.EULinksCheckerAddIn.AddNewLink();
        }
        /// <summary>
        /// Event handler for the Credentials button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCredentials_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.EULinksCheckerAddIn.EditCredentials();
        }
    }
}
