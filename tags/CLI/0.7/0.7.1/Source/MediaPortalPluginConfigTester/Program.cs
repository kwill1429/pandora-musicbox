using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.MediaPortalPlugin.Config;

namespace MediaPortalPluginConfigTester {
    class Program {
        [STAThreadAttribute]
        static void Main(string[] args) {
            System.Windows.Forms.Application.EnableVisualStyles();

            ConfigConnector plugin = new ConfigConnector();
            plugin.ShowPlugin();

        }
    }
}
