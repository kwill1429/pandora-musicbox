using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Data;
using System.Reflection;
using PandoraMusicBox.MediaPortalPlugin.Tools;
using PandoraMusicBox.Engine.Encryption;

namespace PandoraMusicBox.MediaPortalPlugin {

    internal class MusicBoxSettings: BaseSettings {
        BlowfishCipher cipher = new BlowfishCipher(PandoraCryptKeys.In);

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
            get {
                try { return cipher.Decrypt(EncryptedPassword); }
                catch (Exception) {}
                return "";
            }
            set {
                if (value != null) EncryptedPassword = cipher.Encrypt(value);
            }
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
