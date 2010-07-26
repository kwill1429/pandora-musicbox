using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using System.Threading;
using PandoraMusicBox.Engine.Data;
using System.Diagnostics;

namespace PandoraMusicBox.MediaPortalPlugin.Config {
    public partial class LoginForm : Form {

        delegate void InvokeDelegate();

        private Thread verifyLoginThread;
        private Color defaultLabelColor;
        private bool verified = false;

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance;  }
        }

        public LoginForm() {
            InitializeComponent();
        }

        private void VerifyLogin() {
            // set the status label to inform the user of whats happening
            statusLabel.ForeColor = defaultLabelColor;
            statusLabel.Text = "Verifying login credentials...";
            statusLabel.Visible = true;
            upgradeLinkLabel.Visible = false;

            // disable the log in button so the user doesnt double tap it
            okButton.Enabled = false;

            ThreadStart actions = delegate {
                Core.MusicBox.Login(emailTextBox.Text, passwordTextBox.Text);
                LoginDone();
            };

            verifyLoginThread = new Thread(actions);
            verifyLoginThread.Name = "Authentication Thread";
            verifyLoginThread.IsBackground = true;
            verifyLoginThread.Start();
        }

        private void LoginDone() {
            if (InvokeRequired) {
                Invoke(new InvokeDelegate(LoginDone));
                return;
            }

            if (Core.MusicBox.User == null) {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Invalid username or password.";
                statusLabel.Visible = true;

                upgradeLinkLabel.Visible = false;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
            else if (Core.MusicBox.User.AccountType == AccountType.BASIC) {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Pandora One account is required.";
                statusLabel.Visible = true;

                upgradeLinkLabel.Visible = true;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
            else
            {
                statusLabel.ForeColor = Color.Green;
                statusLabel.Text = "Account successfully validated!";
                statusLabel.Visible = true;

                upgradeLinkLabel.Visible = false;

                okButton.Text = "OK";
                okButton.Enabled = true;
                verified = true;
            }
        }

        private void ClearStatus() {
            if (verified) {
                statusLabel.Visible = false;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            emailTextBox.Text = Core.Settings.UserName;
            passwordTextBox.Text = Core.Settings.Password;
            defaultLabelColor = statusLabel.ForeColor;

            // if there are settings already loaded, go ahead and try to validate
            if (!string.IsNullOrEmpty(Core.Settings.UserName) &&
                !string.IsNullOrEmpty(Core.Settings.Password)) {
                    
                VerifyLogin();
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.DialogResult == DialogResult.OK) {
                Core.Settings.UserName = emailTextBox.Text;
                Core.Settings.Password = passwordTextBox.Text;
            }
        }

        private void okButton_Click(object sender, EventArgs e) {
            if (!verified)
                VerifyLogin();
            else {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void emailTextBox_TextChanged(object sender, EventArgs e) {
            ClearStatus();
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e) {
            ClearStatus();
        }

        private void upgradeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ProcessStartInfo processInfo = new ProcessStartInfo("http://www.pandora.com/pandora_one");
            Process.Start(processInfo);
        }


    }
}
