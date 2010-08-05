using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using PandoraMusicBox.Engine.Data;
using System.Diagnostics;

namespace PandoraMusicBox.MediaPortalPlugin.GUI {
    public class MusicBoxGUI: GUIWindow {

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance; }
        }

        bool initialized = false;
        bool playingRadio = false;

        public void LoginAndPlay() {
            // if needed, attempt to log in
            if (Core.MusicBox.User == null)
                Core.MusicBox.Login(Core.Settings.UserName, Core.Settings.Password);

            // if nothing else is playing, play next track in our queue
            if (!g_Player.Playing)
                PlayNextTrack();
        }

        public void PlayNextTrack() {
            if (!initialized)
                return;

            playingRadio = true;

            // grab the next song and have MediaPortal start streaming it
            PandoraSong song = Core.MusicBox.GetNextSong();
            g_Player.PlayAudioStream(song.AudioURL);

            // publish song details to the skin
            SetProperty("#Play.Current.Title", song.Title);
            SetProperty("#Play.Current.Artist", song.Artist);
        }

        private void SetProperty(string property, string value) {
            GUIPropertyManager.SetProperty(property, String.IsNullOrEmpty(value) ? " " : value);
        }

        #region GUIWindow Methods

        public override int GetID {
            get {
                return 82341;
            }
        }

        public override bool Init() {
            base.Init();
            if (!Load(GUIGraphicsContext.Skin + @"\musicbox.xml"))
                return false;

            Core.Initialize();
            
            g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackChanged += new g_Player.ChangedHandler(OnPlayBackChanged);

            initialized = true;
            return true;
        }

        public override void DeInit() {
            Core.Shutdown();

            g_Player.PlayBackEnded -= new g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStopped -= new g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackChanged -= new g_Player.ChangedHandler(OnPlayBackChanged);

            initialized = false;
            base.DeInit();
        }

        protected override void OnPageLoad() {
            base.OnPageLoad();

            LoginAndPlay();
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
                    if (!playingRadio)
                        PlayNextTrack();
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
            showMainContext();
            base.OnShowContextMenu();
        }

        private void OnPlayBackEnded(g_Player.MediaType type, string filename) {
            if (playingRadio) PlayNextTrack();
        }
        
        private void OnPlayBackStopped(g_Player.MediaType type, int stoptime, string filename) {
            playingRadio = false;
        }

        private void OnPlayBackChanged(g_Player.MediaType type, int stoptime, string filename) {
            playingRadio = false;
        }
        
        #endregion


        #region Context Menu Methods

        private void showMainContext()
        {
            IDialogbox dialog = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dialog.Reset();
            dialog.SetHeading("Pandora MusicBox");

            GUIListItem stationsItem = new GUIListItem("Switch Stations...");
            dialog.Add(stationsItem);

            GUIListItem newStationItem = new GUIListItem("Create a New Station...");
            dialog.Add(newStationItem);

            GUIListItem whySelectedItem = new GUIListItem("Why was this song selected?");
            dialog.Add(whySelectedItem);

            GUIListItem moveSongItem = new GUIListItem("Move Song to Another Station...");
            dialog.Add(moveSongItem);

            GUIListItem tempBanItem = new GUIListItem("Temporarily Ban This Song (One Month)");
            dialog.Add(tempBanItem);
            dialog.DoModal(GUIWindowManager.ActiveWindow);

            if (dialog.SelectedId == stationsItem.ItemId)
            {
                PandoraStation newStation = showStationChooser();
                if (newStation != null && newStation != Core.MusicBox.CurrentStation)
                {
                    Core.MusicBox.CurrentStation = newStation;
                    PlayNextTrack();
                }
            }

            else if (dialog.SelectedId == newStationItem.ItemId)
            {
                // todo: onscreen keyboard to create a new station
            }

            else if (dialog.SelectedId == whySelectedItem.ItemId)
            {
                // todo: show the reason song was selected
            }

            else if (dialog.SelectedId == moveSongItem.ItemId)
            {
                PandoraStation newStation = showStationChooser();
                if (newStation != null)
                {
                    // todo: move song to new station
                }
            }

            else if (dialog.SelectedId == tempBanItem.ItemId)
            {
                Core.MusicBox.TemporarilyBanSong();
                PlayNextTrack();
            }
        }

        /// <summary>
        /// Show a station picker
        /// </summary>
        /// <returns>Station chosen by the user, or null if the user canceled out of the menu</returns>
        private PandoraStation showStationChooser()
        {
            IDialogbox dialog = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dialog.Reset();
            dialog.SetHeading("Choose Stations");
            int index = 0;
            Dictionary<int, PandoraStation> stationLookup = new Dictionary<int, PandoraStation>();
            foreach (PandoraStation currStation in Core.MusicBox.AvailableStations)
            {
                if (currStation.IsQuickMix) continue;

                index++;
                stationLookup[index] = currStation;
            }

            foreach (var station in stationLookup)
            {
                GUIListItem listItem = new GUIListItem(station.Value.Name);
                listItem.ItemId = station.Key;
                if (station.Value == Core.MusicBox.CurrentStation)
                    listItem.Selected = true;
                
                dialog.Add(listItem);
            }
            dialog.DoModal(GUIWindowManager.ActiveWindow);
            if (dialog.SelectedId < 0) return null;  // user canceled out of menu

            return stationLookup[dialog.SelectedId];
        }

        #endregion


    }
}
