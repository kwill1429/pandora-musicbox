using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using PandoraMusicBox.Engine.Data;
using System.Diagnostics;
using NLog;
using MediaPortal.Dialogs;
using PandoraMusicBox.Engine;

namespace PandoraMusicBox.MediaPortalPlugin.GUI {
    public class MusicBoxGUI: GUIWindow {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MusicBoxCore Core {
            get { return MusicBoxCore.Instance; }
        }

        private bool PlayingRadio {
            get {
                return g_Player.Playing && 
                       Core.MusicBox.CurrentSong != null && 
                       g_Player.CurrentFile == Core.MusicBox.CurrentSong.AudioURL;
            }
        }

        bool initialized = false;
        bool globalActionListenerInitialized = false;
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

        [SkinControl(13)]
        protected GUILabelControl workingLabel = null;

        #endregion

        public void LoginAndPlay() {
            if (!initialized) return;

            try {
                // if needed, attempt to log in
                if (Core.MusicBox.User == null) {
                    setWorkingAnimationStatus(true);
                    logger.Info("Attempting to log in: " + Core.Settings.UserName);

                    Core.MusicBox.Login(Core.Settings.UserName, Core.Settings.Password);

                    // if a previous station is stored in our settings, attempt to load it
                    if (Core.Settings.LastStation != null)
                        Core.MusicBox.CurrentStation = Core.Settings.LastStation;


                    if (Core.MusicBox.User == null) {
                        logger.Error("Invalid username or password.");
                    }
                }

                // if nothing else is playing, play next track in our queue
                if (!g_Player.Playing)
                    PlayNextTrack();

                setWorkingAnimationStatus(false);
            }
            catch (Exception ex) { GracefullyFail(ex); }
        }

        public void PlayNextTrack() {
            if (!initialized) return;

            PlayNextTrack(false);
        }

        private void PlayNextTrack(bool ignoreSkip) {
            if (!initialized) return;

            try {
                setWorkingAnimationStatus(true);

                // if this is a skip event, check if it is allowed and notify the user if it is not
                bool isSkip = PlayingRadio && !ignoreSkip;
                if (isSkip && !Core.MusicBox.CanSkip()) {
                    logger.Info("User is not currently allowed to skip tracks.");

                    ShowMessage("Pandora",
                        "Unfortunately our music licenses force",
                        "us to limit the number of songs you may",
                        "skip. If want to hear something else,",
                        "try switching to another station.");

                    return;
                }

                if (isSkip) logger.Info("Skipping Current Track");
                logger.Debug("Attempting to Start Next Track");

                // grab the next song and have MediaPortal start streaming it
                lastStartTime = DateTime.Now;
                PandoraSong song = Core.MusicBox.GetNextSong(isSkip);
                g_Player.PlayAudioStream(song.AudioURL);

                logger.Info("Started: '" + song.Title + "' by " + song.Artist);

                UpdateGUI();
                setWorkingAnimationStatus(false);
            }
            catch (Exception ex) { GracefullyFail(ex); }
        }

        public void PromptAndChangeStation() {
            if (!initialized) return;

            try {
                PandoraStation newStation = ShowStationChooser();
                if (newStation != null && newStation != Core.MusicBox.CurrentStation) {
                    setWorkingAnimationStatus(true);

                    Core.MusicBox.CurrentStation = newStation;
                    Core.Settings.LastStation = newStation;
                    PlayNextTrack(true);
                    setWorkingAnimationStatus(false);

                    logger.Info("Switched Station: " + newStation.Name);
                }
            }
            catch (Exception ex) { GracefullyFail(ex); }
        }

        private void GracefullyFail(Exception ex) {
            try {
                logger.ErrorException("Unexpected error!", ex);

                g_Player.Play(Core.Settings.SadTrombone);
                GUIWindowManager.ShowPreviousWindow();
                DeInit();
            } 
            catch (Exception ex2) {
                logger.ErrorException("Failed cleaning up after an error!", ex2);
            }

            ShowMessage("Oops!",
                "Pandora MusicBox has encountered an",
                "unexpected error! Please pardon our",
                "growing pains!",
                 ex == null ? null : ex.Message);
        }

        private void UpdateGUI() {
            if (!initialized) return;

            try {
                var currentSong = Core.MusicBox.CurrentSong;
                if (currentSong == null) return;

                // publish song details to the skin
                SetProperty("#Play.Current.Title", currentSong.Title);
                SetProperty("#Play.Current.Artist", currentSong.Artist);
                SetProperty("#Play.Current.Thumb", currentSong.AlbumArtLargeURL);

                SetProperty("#PandoraMusicBox.Current.Artist", currentSong.Artist);
                SetProperty("#PandoraMusicBox.Current.Title", currentSong.Title);
                SetProperty("#PandoraMusicBox.Current.Album", currentSong.Album);
                SetProperty("#PandoraMusicBox.Current.ArtworkURL", currentSong.AlbumArtLargeURL);
                SetProperty("#PandoraMusicBox.Current.IsAdvertisement", currentSong.IsAdvertisement.ToString());
                if (currentSong.TemporarilyBanned)
                    SetProperty("#PandoraMusicBox.Current.Rating", "TemporarilyBanned");
                else
                    SetProperty("#PandoraMusicBox.Current.Rating", currentSong.Rating.ToString());

                for (int i = 1; i <= 4; i++) {
                    SetProperty("#PandoraMusicBox.History" + i + ".Artist", "");
                    SetProperty("#PandoraMusicBox.History" + i + ".Title", "");
                    SetProperty("#PandoraMusicBox.History" + i + ".Album", "");
                    SetProperty("#PandoraMusicBox.History" + i + ".ArtworkURL", "");
                    SetProperty("#PandoraMusicBox.History" + i + ".IsAdvertisement", "");
                    SetProperty("#PandoraMusicBox.History" + i + ".Rating", "");
                }

                int iHistory = 1;
                foreach (var song in Core.MusicBox.PreviousSongs) {
                    SetProperty("#PandoraMusicBox.History" + iHistory + ".Artist", song.Artist);
                    SetProperty("#PandoraMusicBox.History" + iHistory + ".Title", song.Title);
                    SetProperty("#PandoraMusicBox.History" + iHistory + ".Album", song.Album);
                    SetProperty("#PandoraMusicBox.History" + iHistory + ".ArtworkURL", song.AlbumArtLargeURL);
                    SetProperty("#PandoraMusicBox.History" + iHistory + ".IsAdvertisement", song.IsAdvertisement.ToString());
                    if (song.TemporarilyBanned)
                        SetProperty("#PandoraMusicBox.History" + iHistory + ".Rating", "TemporarilyBanned");
                    else
                        SetProperty("#PandoraMusicBox.History" + iHistory + ".Rating", song.Rating.ToString());
                    iHistory++;
                }

                SetProperty("#PandoraMusicBox.CurrentStation.Name", Core.MusicBox.CurrentStation.Name);

                btnHistory1Song.Visible = (Core.MusicBox.PreviousSongs.Count >= 1);
                btnHistory2Song.Visible = (Core.MusicBox.PreviousSongs.Count >= 2);
                btnHistory3Song.Visible = (Core.MusicBox.PreviousSongs.Count >= 3);
                btnHistory4Song.Visible = (Core.MusicBox.PreviousSongs.Count >= 4);
            }
            catch (Exception ex) { GracefullyFail(ex); }
        }

        private void SetProperty(string property, string value) {
            GUIPropertyManager.SetProperty(property, String.IsNullOrEmpty(value) ? " " : value);
        }

        private void setWorkingAnimationStatus(bool visible) {
            if (workingLabel != null) {
                workingLabel.Visible = visible;
            }
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
            if (!Load(GUIGraphicsContext.Skin + @"\PandoraMusicBox.xml")) {
                logger.Error("Missing PandoraMusicBox.xml skin file!");
                return false;
            }

            try {
                Core.Initialize();

                g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
                initialized = true;
                return true;
            }
            catch (Exception ex) { 
                GracefullyFail(ex);
                return false;
            }

        }

        public override void DeInit() {
            if (!initialized) return;

            try {
                logger.Info("Deinitializing GUI");
                Core.Shutdown();

                g_Player.PlayBackEnded -= new g_Player.EndedHandler(OnPlayBackEnded);
                GUIGraphicsContext.OnNewAction -= new OnActionHandler(OnActionGlobal);

                initialized = false;
                globalActionListenerInitialized = false;
                base.DeInit();
            }
            catch (Exception ex) { logger.ErrorException("Failed deinitializing plugin.", ex); }
        }

        protected override void OnPageLoad() {
            base.OnPageLoad();

            if (!initialized) {
                logger.Warn("Attempting to load window before plugin is initialized.");
                Init();
            }

            logger.Info("Opening Plugin Window");

            if (!globalActionListenerInitialized) {
                globalActionListenerInitialized = true;
                GUIGraphicsContext.OnNewAction += new OnActionHandler(OnActionGlobal);
            }

            LoginAndPlay();
            UpdateGUI();
            setWorkingAnimationStatus(false);
        }

        protected override void OnPageDestroy(int newWindowId) {
            base.OnPageDestroy(newWindowId);

            logger.Info("Closing Plugin Window");
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (!initialized) return;

            try {
                if (controlId == btnCurrentSong.GetID)
                    ShowSongContext(Core.MusicBox.CurrentSong);

                else if (controlId == btnHistory1Song.GetID)
                    ShowSongContext(Core.MusicBox.PreviousSongs[0]);

                else if (controlId == btnHistory2Song.GetID)
                    ShowSongContext(Core.MusicBox.PreviousSongs[1]);

                else if (controlId == btnHistory3Song.GetID)
                    ShowSongContext(Core.MusicBox.PreviousSongs[2]);

                else if (controlId == btnHistory4Song.GetID)
                    ShowSongContext(Core.MusicBox.PreviousSongs[3]);
            }
            catch (Exception ex) { GracefullyFail(ex); }
        }

        // receives actions only when the plugin window is open
        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            try {
                if (!initialized) return;

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
                        if (!PlayingRadio) PlayNextTrack();
                        break;
                    default:
                        base.OnAction(action);
                        break;
                }
            }
            catch (Exception ex) {
                GracefullyFail(ex);
            }
        }

        // receives actions regardless of whether plugin window is open or closed
        private void OnActionGlobal(MediaPortal.GUI.Library.Action action) {
            try {
                if (!initialized) return;

                switch (action.wID) {
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM:
                        logger.Debug("ACTION_NEXT_ITEM fired.");
                        if (PlayingRadio && !Core.MusicBox.CurrentSong.IsAdvertisement) {
                            PlayNextTrack();
                        }
                        break;
                }
            }
            catch (Exception ex) {
                GracefullyFail(ex);
            }
        }

        public override bool OnMessage(GUIMessage message) {
            return base.OnMessage(message);
        }

        protected override void OnShowContextMenu() {
            PromptAndChangeStation();
            base.OnShowContextMenu();
        }

        private void OnPlayBackEnded(g_Player.MediaType type, string filename) {
            try {
                if (!initialized) return;

                logger.Debug("OnPlayBackEnded fired: " + filename);
                if (Core.MusicBox.CurrentSong != null && filename == Core.MusicBox.CurrentSong.AudioURL) {
                    setWorkingAnimationStatus(true);
                    PlayNextTrack();
                    setWorkingAnimationStatus(false);
                }
            }
            catch (Exception ex) {
                GracefullyFail(ex);
            }
        }

        public void ShowMessage(string heading, string line1, string line2, string line3, string line4) {
            GUIDialogOK dialog = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dialog.Reset();
            dialog.SetHeading(heading);
            if (line1 != null) dialog.SetLine(1, line1);
            if (line2 != null) dialog.SetLine(2, line2);
            if (line3 != null) dialog.SetLine(3, line3);
            if (line4 != null) dialog.SetLine(4, line4);
            dialog.DoModal(GetID);
        }

        #endregion


        #region Context Menu Methods

        private void ShowMainContext() {
            IDialogbox dialog = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dialog.Reset();
            dialog.SetHeading("Pandora MusicBox");

            GUIListItem stationsItem = new GUIListItem("Switch Stations...");
            dialog.Add(stationsItem);

            GUIListItem newStationItem = new GUIListItem("Create a New Station...");
            dialog.Add(newStationItem);

            GUIListItem showSongMenuItem = new GUIListItem("Song Options...");
            dialog.Add(showSongMenuItem);

            dialog.DoModal(GUIWindowManager.ActiveWindow);

            if (dialog.SelectedId == stationsItem.ItemId) {
                PromptAndChangeStation();
            }

            else if (dialog.SelectedId == newStationItem.ItemId) {
                // todo: onscreen keyboard to create a new station
            }

            else if (dialog.SelectedId == showSongMenuItem.ItemId) {
                ShowSongContext(Core.MusicBox.CurrentSong);
            }
        }

        private void ShowSongContext(PandoraSong song) {
            if (song.IsAdvertisement) return;

            IDialogbox dialog = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dialog.Reset();
            dialog.SetHeading("Song Options - " + song.Title);

            GUIListItem thumbsUpItem = new GUIListItem("I like this song.");
            dialog.Add(thumbsUpItem);

            GUIListItem thumbsDownItem = new GUIListItem("I don't like this song.");
            dialog.Add(thumbsDownItem);

            GUIListItem tempBanItem = new GUIListItem("I am tired of this song.");
            dialog.Add(tempBanItem);
            dialog.DoModal(GUIWindowManager.ActiveWindow);

            GUIListItem whySelectedItem = new GUIListItem("Why was this song selected?");
            //dialog.Add(whySelectedItem);

            GUIListItem moveSongItem = new GUIListItem("Move Song to Another Station...");
            dialog.Add(moveSongItem);

            if (dialog.SelectedId == thumbsUpItem.ItemId) {
                Core.MusicBox.RateSong(Engine.PandoraRating.Love, song);
                UpdateGUI();
            }

            else if (dialog.SelectedId == thumbsDownItem.ItemId) {
                Core.MusicBox.RateSong(Engine.PandoraRating.Hate, song);
                if (song == Core.MusicBox.CurrentSong)
                    PlayNextTrack();
                UpdateGUI();
            }

            else if (dialog.SelectedId == whySelectedItem.ItemId) {
                // todo: show the reason song was selected
            }

            else if (dialog.SelectedId == moveSongItem.ItemId) {
                PandoraStation newStation = ShowStationChooser();
                if (newStation != null) {
                    // todo: move song to new station
                }
            }

            else if (dialog.SelectedId == tempBanItem.ItemId) {
                Core.MusicBox.TemporarilyBanSong(song);
                if (song == Core.MusicBox.CurrentSong)
                    PlayNextTrack();
                UpdateGUI();
            }
        }


        /// <summary>
        /// Show a station picker
        /// </summary>
        /// <returns>Station chosen by the user, or null if the user canceled out of the menu</returns>
        private PandoraStation ShowStationChooser() {
            IDialogbox dialog = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dialog.Reset();
            dialog.SetHeading("Choose Station");
            int index = 0;

            Dictionary<int, PandoraStation> stationLookup = new Dictionary<int, PandoraStation>();
            foreach (PandoraStation currStation in Core.MusicBox.AvailableStations) {
                if (currStation.IsQuickMix) continue;

                index++;
                stationLookup[index] = currStation;
            }

            foreach (var station in stationLookup) {
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
