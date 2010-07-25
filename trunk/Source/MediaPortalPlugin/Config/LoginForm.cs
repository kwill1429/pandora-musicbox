using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaPortal.GUI.Library;

namespace PandoraMusicBox.MediaPortalPlugin.Config {
    public partial class LoginForm : Form {
        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance;  }
        }

        public LoginForm() {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            emailTextBox.Text = Core.Settings.UserName;
            passwordTextBox.Text = Core.Settings.EncryptedPassword;
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (DialogResult == System.Windows.Forms.DialogResult.OK) {
                Core.Settings.UserName = emailTextBox.Text;
                Core.Settings.EncryptedPassword = passwordTextBox.Text;
            }
        }


    }
}
