namespace EUCases.EULinksCheckerWordAddIn.Forms
{
    partial class fCite
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
            this.btCopy = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.memo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btCopy
            // 
            this.btCopy.Location = new System.Drawing.Point(68, 197);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(95, 30);
            this.btCopy.TabIndex = 0;
            this.btCopy.Text = "Copy";
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(209, 198);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(89, 29);
            this.btClose.TabIndex = 1;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // memo
            // 
            this.memo.Location = new System.Drawing.Point(24, 12);
            this.memo.Name = "memo";
            this.memo.ReadOnly = true;
            this.memo.Size = new System.Drawing.Size(338, 170);
            this.memo.TabIndex = 2;
            this.memo.Text = "";
            // 
            // fCite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 235);
            this.Controls.Add(this.memo);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btCopy);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fCite";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Cite";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fCite_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fCite_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.RichTextBox memo;
    }
}