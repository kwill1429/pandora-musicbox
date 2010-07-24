using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandoraMusicBox.MediaPortalPlugin {
    internal class MusicBoxCore {
        public static MusicBoxCore Instance {
            get {
                if (_instance == null)
                    _instance = new MusicBoxCore();

                return _instance;
            }
        } private static MusicBoxCore _instance;

        public MusicBoxSettings Settings {
            get;
            private set;
        }

        private MusicBoxCore() { }

        public void Initialize() {
            Settings = new MusicBoxSettings();
            Settings.LoadSettings();
        }

        public void Shutdown() {
            this.Settings.SaveSettings();
        }
    }
}
