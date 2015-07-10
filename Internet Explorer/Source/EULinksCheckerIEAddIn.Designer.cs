using BandObjectsLib;
namespace EUCases
{
    partial class EULinksCheckerIEAddIn
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnProcessText = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveLink = new System.Windows.Forms.ToolStripButton();
            this.btnAddNewLink = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveCurrentLink = new System.Windows.Forms.ToolStripButton();
            this.btnSaveToXML = new System.Windows.Forms.ToolStripButton();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.toolStrip.Size = new System.Drawing.Size(645, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip";
            this.toolStrip.Renderer = new NoBorderToolStripRenderer();
            
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnProcessText,
            this.btnRemoveLink,
            this.btnAddNewLink,
            this.btnRemoveCurrentLink,
            this.btnSaveToXML,
            this.btnSettings});            
            // 
            // btnProcessText
            // 
            this.btnProcessText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnProcessText.Image = global::EUCases.Properties.Resources.CheckForLinks;
            this.btnProcessText.ImageTransparentColor = System.Drawing.Color.White;
            this.btnProcessText.Name = "btnProcessText";
            this.btnProcessText.Size = new System.Drawing.Size(105, 22);
            this.btnProcessText.Text = "Check for links";
            this.btnProcessText.Click += new System.EventHandler(this.btnProcessText_Click);
            // 
            // btnRemoveLink
            // 
            this.btnRemoveLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRemoveLink.Image = global::EUCases.Properties.Resources.RemoveAllLinks;
            this.btnRemoveLink.ImageTransparentColor = System.Drawing.Color.White;
            this.btnRemoveLink.Name = "btnRemoveLink";
            this.btnRemoveLink.Size = new System.Drawing.Size(97, 22);
            this.btnRemoveLink.Text = "Remove links";
            this.btnRemoveLink.Click += new System.EventHandler(this.btnRemoveAllLinks_Click);
            // 
            // btnAddNewLink
            // 
            this.btnAddNewLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAddNewLink.Image = global::EUCases.Properties.Resources.AddNewLink;
            this.btnAddNewLink.ImageTransparentColor = System.Drawing.Color.White;
            this.btnAddNewLink.Name = "btnAddNewLink";
            this.btnAddNewLink.Size = new System.Drawing.Size(78, 22);
            this.btnAddNewLink.Text = "Insert link";
            this.btnAddNewLink.Click += new System.EventHandler(this.btnAddNewLink_Click);
            // 
            // btnRemoveCurrentLink
            // 
            this.btnRemoveCurrentLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRemoveCurrentLink.Image = global::EUCases.Properties.Resources.RemoveLink;
            this.btnRemoveCurrentLink.ImageTransparentColor = System.Drawing.Color.White;
            this.btnRemoveCurrentLink.Name = "btnRemoveCurrentLink";
            this.btnRemoveCurrentLink.Size = new System.Drawing.Size(176, 22);
            this.btnRemoveCurrentLink.Text = "Remove links from selection";
            this.btnRemoveCurrentLink.Click += new System.EventHandler(this.btnRemoveCurrentLink_Click);
            // 
            // btnSaveToXML
            // 
            this.btnSaveToXML.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSaveToXML.Image = global::EUCases.Properties.Resources.SaveXml;
            this.btnSaveToXML.ImageTransparentColor = System.Drawing.Color.White;
            this.btnSaveToXML.Name = "btnSaveToXML";
            this.btnSaveToXML.Size = new System.Drawing.Size(87, 22);
            this.btnSaveToXML.Text = "Save to xml";
            this.btnSaveToXML.Click += new System.EventHandler(this.btnSaveToXML_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSettings.Image = global::EUCases.Properties.Resources.Credentials;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.White;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(69, 22);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnCredentials_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            // 
            // EULinksCheckerIEAddIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.toolStrip);
            this.Name = "EULinksCheckerIEAddIn";
            this.Size = new System.Drawing.Size(645, 25);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnProcessText;
        private System.Windows.Forms.ToolStripButton btnRemoveCurrentLink;
        private System.Windows.Forms.ToolStripButton btnRemoveLink;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripButton btnAddNewLink;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnSaveToXML;
    }
}
