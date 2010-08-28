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
using NLog;
using PandoraMusicBox.Engine;

namespace PandoraMusicBox.MediaPortalPlugin.Config {
    public partial class LoginForm : Form {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        delegate void InvokeDelegate();
        delegate void InvokeDelegateEx(PandoraException ex);

        private Thread verifyLoginThread;
        private Color defaultLabelColor;
        private bool verified = false;

        //private readonly string UPGRADE = "http://www.pandora.com/pandora_one";
        private readonly string LICENSE = "http://www.pandora.com/restricted";
        private string link;

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance;  }
        }

        public LoginForm() {
            InitializeComponent();
        }

        private void VerifyLogin() {
            logger.Info("Attepting to verify user: " + emailTextBox.Text);

            // set the status label to inform the user of whats happening
            statusLabel.ForeColor = defaultLabelColor;
            statusLabel.Text = "Verifying login credentials...";
            statusLabel.Visible = true;
            moreInfoLinkLabel.Visible = false;

            // disable the log in button so the user doesnt double tap it
            okButton.Enabled = false;

            ThreadStart actions = delegate {
                try { Core.MusicBox.Login(emailTextBox.Text, passwordTextBox.Text); }
                catch (PandoraException ex) { 
                    LoginFailed(ex);
                    return;
                }

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
                logger.Info("Invalid username or password.");

                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Invalid username or password.";
                statusLabel.Visible = true;

                moreInfoLinkLabel.Visible = false;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
            /*
            else if (Core.MusicBox.User.AccountType == AccountType.BASIC) {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Pandora One account is required.";
                statusLabel.Visible = true;

                upgradeLinkLabel.Visible = true;
                link = UPGRADE;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
            */
            else {
                logger.Info("Verified credentials.");

                statusLabel.ForeColor = Color.Green;
                statusLabel.Text = "Account successfully validated!";
                statusLabel.Visible = true;

                moreInfoLinkLabel.Visible = false;

                okButton.Text = "OK";
                okButton.Enabled = true;
                verified = true;
            }
        }

        private void LoginFailed(PandoraException ex) {
            if (InvokeRequired) {
                Invoke(new InvokeDelegateEx(LoginFailed), new object[] { ex });
                return;
            }

            okButton.Text = "Sign In";
            okButton.Enabled = true;
            verified = false;

            if (ex.ErrorCode == ErrorCodeEnum.LICENSE_RESTRICTION) {
                MessageBox.Show("We are deeply, deeply sorry to say that due to licensing constraints, we can no longer allow access to Pandora for listeners located outside of the U.S. We will continue to work diligently to realize the vision of a truly global Pandora, but for the time being we are required to restrict it's use. We are very sad to have to do this, but there is no other alternative.");
                logger.Error(ex.Message);

                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Invalid Region.";

                moreInfoLinkLabel.Visible = true;
                link = LICENSE;
                return;
            }

            statusLabel.ForeColor = Color.Red;
            statusLabel.Text = "Unexpected Error.";
            logger.ErrorException("Unexpected Error!", ex);
        }

        private void ClearStatus() {
            if (verified) {
                statusLabel.Visible = false;

                okButton.Text = "Sign In";
                okButton.Enabled = true;
                verified = false;
            }
        }

        private void CenterDialog() {
            Rectangle screen = Screen.GetWorkingArea(this);

            this.Location = new Point(((screen.Width - this.Width) / 2) + screen.X,
                                      ((screen.Height - this.Height) / 2) + screen.Y);
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            logger.Info("Opening Login Screen");

            CenterDialog();

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
            string message;

            if (this.DialogResult == DialogResult.OK) {
                message = "Saved new login settings.";
                Core.Settings.UserName = emailTextBox.Text;
                Core.Settings.Password = passwordTextBox.Text;
            }
            else {
                message = "Discarded new login settings.";
            }

            logger.Info("Closing Login Screen: " + message);
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
            ProcessStartInfo processInfo = new ProcessStartInfo(link);
            Process.Start(processInfo);
        }
    }
}
