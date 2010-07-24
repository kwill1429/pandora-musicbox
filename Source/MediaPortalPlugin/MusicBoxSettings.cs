using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Data;
using System.Reflection;

namespace PandoraMusicBox.MediaPortalPlugin {

    internal class MusicBoxSettings: BaseSettings {
        public MusicBoxSettings() {
            FileName = "musicbox.xml";
            Namespace = "PandoraMusicBox";
        }

        [Setting]
        public string UserName {
            get;
            set;
        }

        [Setting]
        public string EncryptedPassword {
            get;
            set;
        }

        public string Password {
            get;
            set;
        }

        [Setting]
        public string LastStationId {
            get;
            set;
        }

        public PandoraStation LastStation {
            get;
            set;
        }
    }
}
