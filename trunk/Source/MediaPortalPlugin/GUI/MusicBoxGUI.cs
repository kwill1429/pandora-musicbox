using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.MediaPortalPlugin.GUI {
    public class MusicBoxGUI: GUIWindow {

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance; }
        }

        bool initialized = false;

        #region GUIWindow Methods

        public override int GetID {
            get {
                return 82341;
            }
        }

        public override bool Init() {
            base.Init();
            bool skinLoaded = Load(GUIGraphicsContext.Skin + @"\musicbox.xml");

            if (skinLoaded) {
                Core.Initialize();
            }

            initialized = skinLoaded;
            return skinLoaded;
        }

        public override void DeInit() {
            Core.Shutdown();

            initialized = false;
            base.DeInit();
        }

        protected override void OnPageLoad() {
            base.OnPageLoad();

            // if needed, attempt to log in
            if (Core.MusicBox.User == null)
                Core.MusicBox.Login(Core.Settings.UserName, Core.Settings.Password);

            // if nothing else is playing, play next track in our queue
            if (!g_Player.Playing) {
                PandoraSong song = Core.MusicBox.GetNextSong();
                g_Player.PlayAudioStream(song.AudioURL);
            }
        }

        protected override void OnPageDestroy(int newWindowId) {
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (!initialized)
                return;

            switch (controlId) {
            }
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if (!initialized)
                return;

            switch (action.wID) {
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_HOME:
                    GUIWindowManager.ShowPreviousWindow();
                    break;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
                    GUIWindowManager.ShowPreviousWindow();
                    break;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
                    break;
                default:
                    base.OnAction(action);
                    break;
            }
        }
        
        public override bool OnMessage(GUIMessage message) {
            return base.OnMessage(message);
        }

        protected override void OnShowContextMenu() {
            base.OnShowContextMenu();
        }
        
        #endregion
    }
}
