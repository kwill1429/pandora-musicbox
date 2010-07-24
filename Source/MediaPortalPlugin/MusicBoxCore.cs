using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.MediaPortalPlugin.Properties;

namespace PandoraMusicBox.MediaPortalPlugin {
    internal class MusicBoxCore {
        public static MusicBoxCore Instance {
            get {
                if (_instance == null)
                    _instance = new MusicBoxCore();

                return _instance;
            }
        } private static MusicBoxCore _instance;

        private MusicBoxCore() { }

        public void Initialize() {
            CheckForUpgrade();
        }

        public void Shutdown() {
        }

        private void CheckForUpgrade() {
            if (Settings.Default.UpgradeRequired) {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }
    }
}
