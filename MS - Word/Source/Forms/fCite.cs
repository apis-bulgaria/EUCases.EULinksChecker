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
    public partial class fCite : Form
    {
        public fCite()
        {
            InitializeComponent();
        }
        private void fCite_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) Hide();
        }
        private void btClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void btCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(memo.Text);
        }
        public void Show(string cite,string tit,int type){
            memo.Text = cite;
            Text = tit;
            Visible = true;
        }
        public void SetTxtClose(string s) {
            btClose.Text = s;
        }
        public void SetTxtCopy(string s)
        {
            btCopy.Text = s;
        }
        private void fCite_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
