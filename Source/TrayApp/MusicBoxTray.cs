using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using TrayApp.Properties;

namespace TrayApp {
    public class MusicBoxTray: Form {
        protected NotifyIcon trayIcon;
        protected ContextMenuStrip trayMenu;

        public MusicBoxTray() {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenuStrip();

            Panel p = new Panel();
            Label l = new Label();
            l.Text = "SONG PANEL";
            l.AutoSize = false;
            l.Width = 500;
            p.Controls.Add(l);

            trayMenu.Items.Add(new ToolStripControlHost(p));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Login", null, OnLogin);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Pandora MusicBox";
            trayIcon.Icon = Resources.PandoraLogo;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
        }

        protected override void OnLoad(EventArgs e) {
            Visible = false;
            ShowInTaskbar = false;
            base.OnLoad(e);
        }

        protected void OnExit(object sender, EventArgs e) {
            Application.Exit();
        }
        
        protected void OnLogin(object sender, EventArgs e) {
            MessageBox.Show("login");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MusicBoxTray());
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // MusicBoxTray
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "MusicBoxTray";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

    }
}
