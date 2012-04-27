using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PandoraMusicBox.Engine.Encryption;

namespace PandoraHeaderDecoder {
    public partial class Form1 : Form {
        BlowfishCipher inCipher = new BlowfishCipher(PandoraCryptKeys.In);
        BlowfishCipher outCipher = new BlowfishCipher(PandoraCryptKeys.Out);

        public Form1() {
            InitializeComponent();
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e) {
            eval();
        }

        public void eval() {
            try {
                outKeyTextBox.Text = outCipher.Decrypt(inputTextBox.Text);
                inKeyTextBox.Text = inCipher.Decrypt(inputTextBox.Text);
            }
            catch (Exception) {
                outKeyTextBox.Text = "";
                inKeyTextBox.Text = "";
            }
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.A)
                inputTextBox.SelectAll();
        }


    }
}
