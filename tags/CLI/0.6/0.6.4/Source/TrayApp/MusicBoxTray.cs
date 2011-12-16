using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

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
            trayMenu.Items.Add("Exit", null, OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Pandora MusicBox";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;

            trayIcon.Click += new EventHandler(trayIcon_Click);

        }

        void trayIcon_Click(object sender, EventArgs e) {
            //Visible = true;
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

    }
}
