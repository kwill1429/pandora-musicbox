using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using PandoraMusicBox.MediaPortalPlugin.Properties;

namespace PandoraMusicBox.MediaPortalPlugin.Config {
    public partial class ConfigForm : Form {
        public ConfigForm() {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e) {
            emailTextBox.Text = Settings.Default.Username;
            passwordTextBox.Text = Settings.Default.EncryptedPassword;
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (DialogResult == System.Windows.Forms.DialogResult.OK) {
                emailTextBox.Text = Settings.Default.Username;
                passwordTextBox.Text = Settings.Default.EncryptedPassword;
            }
        }


    }
}
