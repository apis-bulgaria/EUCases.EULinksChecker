namespace EUCases.Resources
{
    partial class fInsertLink
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbHyperlink = new System.Windows.Forms.TextBox();
            this.lblInsertLink = new System.Windows.Forms.Label();
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbHyperlink
            // 
            this.tbHyperlink.Location = new System.Drawing.Point(12, 24);
            this.tbHyperlink.Name = "tbHyperlink";
            this.tbHyperlink.Size = new System.Drawing.Size(260, 20);
            this.tbHyperlink.TabIndex = 0;
            // 
            // lblInsertLink
            // 
            this.lblInsertLink.AutoSize = true;
            this.lblInsertLink.Location = new System.Drawing.Point(13, 5);
            this.lblInsertLink.Name = "lblInsertLink";
            this.lblInsertLink.Size = new System.Drawing.Size(52, 13);
            this.lblInsertLink.TabIndex = 1;
            this.lblInsertLink.Text = "Insert link";
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(25, 60);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(100, 21);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(157, 60);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(100, 21);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // fInsertLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 89);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.lblInsertLink);
            this.Controls.Add(this.tbHyperlink);
            this.Name = "fInsertLink";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fInsertLink";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbHyperlink;
        private System.Windows.Forms.Label lblInsertLink;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
    }
}