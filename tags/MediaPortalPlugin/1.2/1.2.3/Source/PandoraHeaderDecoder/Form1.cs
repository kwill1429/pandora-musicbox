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
        BlowfishCipher cipher = new BlowfishCipher(PandoraCryptKeys.Out);

        public Form1() {
            InitializeComponent();
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e) {
            try {
                outputTextBox.Text = cipher.Decrypt(inputTextBox.Text);
            }
            catch (Exception) {
                outputTextBox.Text = "???";
            }
        }
    }
}
