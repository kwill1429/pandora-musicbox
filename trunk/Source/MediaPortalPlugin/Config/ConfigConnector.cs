using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using System.Windows.Forms;

namespace PandoraMusicBox.MediaPortalPlugin.Config {
    public class ConfigConnector: ISetupForm {
        #region ISetupForm Members

        public string PluginName() {
            return "Pandora MusicBox";
        }

        public string Description() {
            return "Listen to the Pandora music service through MediaPortal.";
        }

        public string Author() {
            return "John Conrad (fforde)";
        }

        public void ShowPlugin() {
            try {
                MusicBoxCore.Instance.Initialize();
                ConfigForm config = new ConfigForm();
                config.ShowDialog();
            }
            catch (Exception) {
                MessageBox.Show("There was an unexpected error in the Pandora MusicBox Configuration screen!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public bool CanEnable() {
            return true;
        }

        public int GetWindowId() {
            return 82341;
        }

        public bool DefaultEnabled() {
            return true;
        }

        public bool HasSetup() {
            return true;
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage) {
            strButtonText = "Pandora";
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = "hover_pandora.png";
            return true;
        }

        #endregion
    }
}
