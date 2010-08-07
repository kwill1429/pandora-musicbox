using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using PandoraMusicBox.Engine.Data;
using System.Diagnostics;
using NLog;

namespace PandoraMusicBox.MediaPortalPlugin.GUI {
    public class MusicBoxGUI: GUIWindow {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance; }
        }

        bool initialized = false;
        bool globalActionListenerInitialized = false;

        bool playingRadio = false;
        DateTime? lastStartTime = null;

        #region GUI Controls

        [SkinControl(2)]
        protected GUIButtonControl btnCurrentSong = null;

        [SkinControl(3)]
        protected GUIButtonControl btnHistory1Song = null;

        [SkinControl(4)]
        protected GUIButtonControl btnHistory2Song = null;

        [SkinControl(5)]
        protected GUIButtonControl btnHistory3Song = null;

        [SkinControl(6)]
        protected GUIButtonControl btnHistory4Song = null;


        #endregion

        public void LoginAndPlay() {
            // if needed, attempt to log in
            if (Core.MusicBox.User == null) {
                logger.Info("Attempting to log in: " + Core.Settings.UserName);
                Core.MusicBox.Login(Core.Settings.UserName, Core.Settings.Password);

                if (Core.MusicBox.User == null) {
                    logger.Error("Invalid username or password.");
                }
            }

            // if nothing else is playing, play next track in our queue
            if (!g_Player.Playing)
                PlayNextTrack();
        }

        public void PlayNextTrack() {
            if (!initialized)
                return;

            logger.Info("Starting Next Track");
            playingRadio = true;

            // grab the next song and have MediaPortal start streaming it
            lastStartTime = DateTime.Now;
            PandoraSong song = Core.MusicBox.GetNextSong();
            g_Player.PlayAudioStream(song.AudioURL);

            UpdateGUI();
        }

        private void UpdateGUI() {
            var currentSong = Core.MusicBox.CurrentSong;

            // publish song details to the skin
            SetProperty("#Play.Current.Title", currentSong.Title);
            SetProperty("#Play.Current.Artist", currentSong.Artist);

            SetProperty("#PandoraMusicBox.Current.Artist", currentSong.Artist);
            SetProperty("#PandoraMusicBox.Current.Title", currentSong.Title);
            SetProperty("#PandoraMusicBox.Current.Album", currentSong.Album);
            SetProperty("#PandoraMusicBox.Current.ArtworkURL", currentSong.ArtworkURL);
            SetProperty("#PandoraMusicBox.Current.Rating", currentSong.Rating.ToString());
            SetProperty("#PandoraMusicBox.Current.TemporarilyBanned", currentSong.TemporarilyBanned.ToString());

            for (int i = 1; i <= 4; i++)
            {
                SetProperty("#PandoraMusicBox.History" + i + ".Artist", "");
                SetProperty("#PandoraMusicBox.History" + i + ".Title", "");
                SetProperty("#PandoraMusicBox.History" + i + ".Album", "");
                SetProperty("#PandoraMusicBox.History" + i + ".ArtworkURL", "");
                SetProperty("#PandoraMusicBox.History" + i + ".Rating", "");
                SetProperty("#PandoraMusicBox.History" + i + ".TemporarilyBanned", "");
            }

            int iHistory = 1;
            foreach (var song in Core.MusicBox.PreviousSongs)
            {
                SetProperty("#PandoraMusicBox.History" + iHistory + ".Artist", song.Artist);
                SetProperty("#PandoraMusicBox.History" + iHistory + ".Title", song.Title);
                SetProperty("#PandoraMusicBox.History" + iHistory + ".Album", song.Album);
                SetProperty("#PandoraMusicBox.History" + iHistory + ".ArtworkURL", song.ArtworkURL);
                SetProperty("#PandoraMusicBox.History" + iHistory + ".Rating", song.Rating.ToString());
                SetProperty("#PandoraMusicBox.History" + iHistory + ".TemporarilyBanned", song.TemporarilyBanned.ToString());
                iHistory++;
            }

            SetProperty("#PandoraMusicBox.CurrentStation.Name", Core.MusicBox.CurrentStation.Name);
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

            logger.Info("Initializing GUI");
            if (!Load(GUIGraphicsContext.Skin + @"\musicbox.xml")) {
                logger.Error("Missing musicbox.xml skin file!");
                return false;
            }

            Core.Initialize();
            
            g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackChanged += new g_Player.ChangedHandler(OnPlayBackChanged);

            initialized = true;
            return true;
        }

        public override void DeInit() {
            logger.Info("Deinitializing GUI");
            Core.Shutdown();

            g_Player.PlayBackEnded -= new g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStopped -= new g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackChanged -= new g_Player.ChangedHandler(OnPlayBackChanged);
            
            GUIGraphicsContext.OnNewAction -= new OnActionHandler(OnActionGlobal);

            initialized = false;
            globalActionListenerInitialized = false;
            base.DeInit();
        }

        protected override void OnPageLoad() {
            base.OnPageLoad();

            logger.Info("Opening Plugin Window");

            if (!globalActionListenerInitialized) {
                globalActionListenerInitialized = true;
                GUIGraphicsContext.OnNewAction += new OnActionHandler(OnActionGlobal);
            }

            LoginAndPlay();
        }

        protected override void OnPageDestroy(int newWindowId) {
            base.OnPageDestroy(newWindowId);

            logger.Info("Closing Plugin Window");
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (!initialized)
                return;

            switch (controlId) {
            }
        }

        // receives actions only when the plugin window is open
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
                    logger.Debug("ACTION_PLAY or ACTION_MUSIC_PLAY fired.");
                    if (!playingRadio)
                        PlayNextTrack();
                    break;
                default:
                    base.OnAction(action);
                    break;
            }
        }

        // receives actions regardless of whether plugin window is open or closed
        private void OnActionGlobal(MediaPortal.GUI.Library.Action action) {
            switch (action.wID) {
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM:
                    logger.Debug("ACTION_NEXT_ITEM fired.");
                    if (playingRadio) {                        
                        PlayNextTrack();                        
                    }
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
            logger.Debug("OnPlayBackEnded fired.");

            // if this was triggered from a recent skip, ignore the event
            if (lastStartTime != null && DateTime.Now - (DateTime)lastStartTime < new TimeSpan(0, 0, 2)) 
                return;            

            if (playingRadio) PlayNextTrack();
        }
        
        private void OnPlayBackStopped(g_Player.MediaType type, int stoptime, string filename) {
            logger.Debug("OnPlayBackStopped fired.");
            playingRadio = false;
        }

        private void OnPlayBackChanged(g_Player.MediaType type, int stoptime, string filename) {
            logger.Debug("OnPlayBackChanged fired.");
            
            // if this was triggered from a recent skip, ignore the event
            if (lastStartTime != null && DateTime.Now - (DateTime)lastStartTime < new TimeSpan(0, 0, 2))
                return;  

            // something else started playing, so remember that pandora is now "turned off"
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
