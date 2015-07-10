namespace EUCases.EULinksCheckerWordAddIn
{
    partial class EULCRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public EULCRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            EULinksCheckerAddIn.InitRes();
            InitializeComponent();
            ApplLng(this);
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EULCRibbon));
            this.tEULC = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.bPutLinksAndTerms = this.Factory.CreateRibbonButton();
            this.bRemoveLinksAndTerms = this.Factory.CreateRibbonButton();
            this.bAddNewLink = this.Factory.CreateRibbonButton();
            this.bRemove1 = this.Factory.CreateRibbonButton();
            this.bSaveToXml = this.Factory.CreateRibbonButton();
            this.bCredentials = this.Factory.CreateRibbonButton();
            this.tEULC.SuspendLayout();
            this.group1.SuspendLayout();
            // 
            // tEULC
            // 
            this.tEULC.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tEULC.Groups.Add(this.group1);
            this.tEULC.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cEULinksChecker;
            this.tEULC.Name = "tEULC";
            // 
            // group1
            // 
            this.group1.Items.Add(this.bPutLinksAndTerms);
            this.group1.Items.Add(this.bRemoveLinksAndTerms);
            this.group1.Items.Add(this.bAddNewLink);
            this.group1.Items.Add(this.bRemove1);
            this.group1.Items.Add(this.bSaveToXml);
            this.group1.Items.Add(this.bCredentials);
            this.group1.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cOperations;
            this.group1.Name = "group1";
            // 
            // bPutLinksAndTerms
            // 
            this.bPutLinksAndTerms.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bPutLinksAndTerms.Image = ((System.Drawing.Image)(resources.GetObject("bPutLinksAndTerms.Image")));
            this.bPutLinksAndTerms.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cCheckFrLinks;
            this.bPutLinksAndTerms.Name = "bPutLinksAndTerms";
            this.bPutLinksAndTerms.ScreenTip = "Recognition and mark-up of references to EU legislation and case law";
            this.bPutLinksAndTerms.ShowImage = true;
            this.bPutLinksAndTerms.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bPutLinksAndTerms_Click);
            // 
            // bRemoveLinksAndTerms
            // 
            this.bRemoveLinksAndTerms.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bRemoveLinksAndTerms.Image = ((System.Drawing.Image)(resources.GetObject("bRemoveLinksAndTerms.Image")));
            this.bRemoveLinksAndTerms.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cRemoveLinks;
            this.bRemoveLinksAndTerms.Name = "bRemoveLinksAndTerms";
            this.bRemoveLinksAndTerms.ScreenTip = "Removal of links to EU legislation and case law set by “Check for links” function" +
    " of EULinksChecker";
            this.bRemoveLinksAndTerms.ShowImage = true;
            this.bRemoveLinksAndTerms.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bRemoveLinksAndTerms_Click);
            // 
            // bAddNewLink
            // 
            this.bAddNewLink.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bAddNewLink.Image = ((System.Drawing.Image)(resources.GetObject("bAddNewLink.Image")));
            this.bAddNewLink.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cInsertLink;
            this.bAddNewLink.Name = "bAddNewLink";
            this.bAddNewLink.ScreenTip = "Inserts a link to external web resource manually";
            this.bAddNewLink.ShowImage = true;
            this.bAddNewLink.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bAddNewLink_Click);
            // 
            // bRemove1
            // 
            this.bRemove1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bRemove1.Image = ((System.Drawing.Image)(resources.GetObject("bRemove1.Image")));
            this.bRemove1.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cRemoveLinksFromSelection;
            this.bRemove1.Name = "bRemove1";
            this.bRemove1.ScreenTip = "Removing links to EU legislation and case law set by EULinksChecker from the sele" +
    "cted text";
            this.bRemove1.ShowImage = true;
            this.bRemove1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bRemove1_Click);
            // 
            // bSaveToXml
            // 
            this.bSaveToXml.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bSaveToXml.Image = ((System.Drawing.Image)(resources.GetObject("bSaveToXml.Image")));
            this.bSaveToXml.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.Save2Xml;
            this.bSaveToXml.Name = "bSaveToXml";
            this.bSaveToXml.ScreenTip = "Save text to XML-file";
            this.bSaveToXml.ShowImage = true;
            this.bSaveToXml.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bSaveToXml_Click);
            // 
            // bCredentials
            // 
            this.bCredentials.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.bCredentials.Image = ((System.Drawing.Image)(resources.GetObject("bCredentials.Image")));
            this.bCredentials.Label = global::EUCases.EULinksCheckerWordAddIn.Resources.Resource.cm_Credentials;
            this.bCredentials.Name = "bCredentials";
            this.bCredentials.ScreenTip = "Settings";
            this.bCredentials.ShowImage = true;
            this.bCredentials.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.bCredentials_Click);
            // 
            // EULCRibbon
            // 
            this.Name = "EULCRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tEULC);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.EULCRibbon_Load);
            this.tEULC.ResumeLayout(false);
            this.tEULC.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();

        }
        #endregion
        internal Microsoft.Office.Tools.Ribbon.RibbonTab tEULC;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bPutLinksAndTerms;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bRemoveLinksAndTerms;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bSaveToXml;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bRemove1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bAddNewLink;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton bCredentials;
    }
    partial class ThisRibbonCollection
    {
        internal EULCRibbon EULCRibbon
        {
            get { return this.GetRibbon<EULCRibbon>(); }
        }
    }
}
