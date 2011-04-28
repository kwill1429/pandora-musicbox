using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    public class MusicBox {

        protected delegate void ExecuteDelegate();

        protected PandoraIO pandora = new PandoraIO();
        protected Queue<PandoraSong> playlist = new Queue<PandoraSong>();

        protected TimeSpan timeSinceLastAd = new TimeSpan(0);
        protected DateTime timeLastSongGrabbed;
        protected TimeSpan? currentAdInterval = null;

        private string[] specialStationTags = new string[] { "(Holiday)", "(Children's)" };

        /// <summary>
        /// The current user that is logged in.
        /// </summary>
        public PandoraUser User {
            get;
            protected set;
        }

        /// <summary>
        /// The current station being listened to.
        /// </summary>
        public PandoraStation CurrentStation {
            get { return _currentStation; }
            set {
                if (AvailableStations.Contains(value)) {
                    _currentStation = value;
                    CurrentSong = null;
                    PreviousSongs.Clear();
                    playlist.Clear();
                    LoadMoreSongs();
                }
            }
        } protected PandoraStation _currentStation;

        /// <summary>
        /// The current song being played.
        /// </summary>
        public PandoraSong CurrentSong {
            get;
            protected set;
        }

        /// <summary>
        /// A list of the last few songs that have been played.
        /// </summary>
        public List<PandoraSong> PreviousSongs {
            get;
            protected set;
        }

        /// <summary>
        /// A list of all available stations for the currently logged in user.
        /// </summary>
        public List<PandoraStation> AvailableStations {
            get;
            protected set;
        }

        /// <summary>
        /// Keeps track of when the user has skipped tracks on various stations and if
        /// they are allowed to skip at the moment.
        /// </summary>
        public SkipHistory SkipHistory {
            get;
            set;
        }

        /// <summary>
        /// If set to true, special station tags like "Holiday" and "Children's" will
        /// be removed from the artist name in song meta data.
        /// </summary>
        public bool RemoveStationTags {
            get { return _removeSpecialStationTag; }
            set { _removeSpecialStationTag = value; }
        } private bool _removeSpecialStationTag = true;

        /// <summary>
        /// Logs into Pandora with the given credentials.
        /// </summary>
        /// <returns>true if the user was successfully logged in.</returns>
        public bool Login(string username, string password) {
            Clear();

            User = pandora.AuthenticateListener(username, password);
            if (User != null && pandora.CanListen(User)) {
                SkipHistory = new SkipHistory(User);
                
                AvailableStations = pandora.GetStations(User);

                // try to grab the first station in the list that is not the quickmix station
                foreach (PandoraStation currStation in AvailableStations)
                    if (!currStation.IsQuickMix) {
                        CurrentStation = currStation;
                        break;
                    }

                return true;
            }

            Clear();
            return false;
        }

        /// <summary>
        /// Clears internal settings reseting the class.
        /// </summary>
        public void Logout() {
            Clear();
        }

        /// <summary>
        /// Returns true if the user is allowed to skip the current track on the current station
        /// </summary>
        /// <returns></returns>
        public bool CanSkip() {
            return SkipHistory.CanSkip(CurrentStation);
        }

        /// <summary>
        /// Returns the next song and updates the CurrentSong property. Will throw a PandoraException if
        /// skipping and the user is nto allowed to skip at this point in time. Call CanSkip() first as needed.
        /// </summary>
        /// <returns></returns>
        public PandoraSong GetNextSong(bool isSkip) {            
            // update the previous songs list
            while (PreviousSongs.Count > 4) 
                PreviousSongs.RemoveAt(4);

            // if necessary log a skip event. this will throw an exception if a skip is not allowed
            if (isSkip) SkipHistory.Skip(CurrentStation);            

            if (CurrentSong != null) {
                // update playback history
                PreviousSongs.Insert(0, CurrentSong);

                // keep track of how much listening time has occured since our last ad
                TimeSpan realDuration = DateTime.Now - (DateTime)timeLastSongGrabbed;
                if (realDuration > CurrentSong.Length)
                    timeSinceLastAd = timeSinceLastAd.Add(CurrentSong.Length);
                else
                    timeSinceLastAd = timeSinceLastAd.Add(realDuration);
            }

            timeLastSongGrabbed = DateTime.Now;

            // if it is time for an ad reset the ad timer and return an ad instead of a song
            if (currentAdInterval == null) currentAdInterval = new TimeSpan(0, User.AdInterval / 2, 0);
            if (User.AccountType == AccountType.BASIC && timeSinceLastAd > currentAdInterval) {
                currentAdInterval = new TimeSpan(0, User.AdInterval, 0);
                timeSinceLastAd = new TimeSpan(0);

                CurrentSong = pandora.GetAdvertisement(User);
                return CurrentSong;
            }


            // grab the next song in our queue. songs become invalid after an 
            // unspecified number of hours.
            do {
                if (playlist.Count < 2) LoadMoreSongs();
                CurrentSong = playlist.Dequeue();
            } while (!pandora.IsValid(CurrentSong));

            pandora.GetSongLength(User, CurrentSong);
            return CurrentSong;
        }

        /// <summary>
        /// Rate the specified song. A positive or negative rating will influence future songs 
        /// played from the current station.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="song"></param>
        public void RateSong(PandoraSong song, PandoraRating rating) {
            VerifyAndExecute(delegate {
                pandora.RateSong(User, CurrentStation, song, rating);
            });
        }

        /// <summary>
        /// Ban this song from playing on any of the users stations for one month.
        /// </summary>
        /// <param name="song"></param>
        public void TemporarilyBanSong(PandoraSong song) {
            VerifyAndExecute(delegate {
                pandora.AddTiredSong(User, song);
            });    
        }

        protected void Clear() {
            if (PreviousSongs == null) PreviousSongs = new List<PandoraSong>();
            if (AvailableStations == null) AvailableStations = new List<PandoraStation>();

            _currentStation = null;
            CurrentSong = null;
            PreviousSongs.Clear();
            AvailableStations.Clear();
            SkipHistory = null;
            User = null;
        }

        protected void LoadMoreSongs() {
            List<PandoraSong> newSongs = new List<PandoraSong>();

            VerifyAndExecute(delegate {
                newSongs = pandora.GetSongs(User, CurrentStation);
            });

            // add our new songs to the playlist
            foreach (PandoraSong currSong in newSongs) {
                pandora.GetLargeArtworkURL(currSong);
                CheckForStationTags(currSong);
                playlist.Enqueue(currSong);
            }
        }

        protected void CheckForStationTags(PandoraSong song) {
            if (!RemoveStationTags)
                return;

            foreach (string currTag in specialStationTags) {
                if (song.Artist.EndsWith(currTag)) {
                    song.Artist = song.Artist.Remove(song.Artist.LastIndexOf(currTag)).Trim();
                    return;
                }
            }
        }

        /// <summary>
        /// Try to execute the supplied logic, if we get an authentication error, relogin and try again.
        /// </summary>
        /// <param name="logic"></param>
        protected void VerifyAndExecute(ExecuteDelegate logic) {
            try { logic(); }
            catch (PandoraException ex) {
                // if there was an error and it wasnt an expired session, just toss it up to the client
                if (ex.ErrorCode != ErrorCodeEnum.AUTH_INVALID_TOKEN) 
                    throw;

                // our login expired, try logging in again
                User = pandora.AuthenticateListener(User.Name, User.Password);
                playlist.Clear();
                if (User == null) throw new PandoraException("Username and/or password are no longer valid!");

                // and again, try the desired action
                logic();
            }            
        }

    }
}
