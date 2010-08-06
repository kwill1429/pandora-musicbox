using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    public class MusicBox {

        protected delegate void ExecuteDelegate();

        protected PandoraIO pandora = new PandoraIO();
        protected Queue<PandoraSong> playlist = new Queue<PandoraSong>();

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
        /// Logs into Pandora with the given credentials.
        /// </summary>
        /// <returns>true if the user was successfully logged in.</returns>
        public bool Login(string username, string password) {
            Clear();

            User = pandora.AuthenticateListener(username, password);
            if (User != null && pandora.CanListen(User)) {
                AvailableStations = pandora.GetStations(User);
                if (AvailableStations.Count > 0) {
                    CurrentStation = AvailableStations[1];
                    LoadMoreSongs();
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
        /// Returns the next song and updates the CurrentSong property. 
        /// </summary>
        /// <returns></returns>
        public PandoraSong GetNextSong() {
            while (PreviousSongs.Count > 4) 
                PreviousSongs.RemoveAt(0);

            if (CurrentSong != null)
                PreviousSongs.Add(CurrentSong);

            if (!pandora.CanListen(User)) 
                playlist.Clear();

            if (playlist.Count < 3)
                LoadMoreSongs();

            CurrentSong = playlist.Dequeue();            
            return CurrentSong;
        }

        /// <summary>
        /// Rate the current song. A positive or negative rating will influence future songs 
        /// played from the current station.
        /// </summary>
        /// <param name="rating"></param>
        public void RateSong(PandoraRating rating) {
            VerifyAndExecute(delegate {
                pandora.RateSong(User, CurrentStation, CurrentSong, rating);
            });
        }

        /// <summary>
        /// Ban this song from playing on any of the users stations for one month.
        /// </summary>
        public void TemporarilyBanSong() {
            VerifyAndExecute(delegate {
                pandora.AddTiredSong(User, CurrentSong);
            });    
        }

        protected void Clear() {
            if (PreviousSongs == null) PreviousSongs = new List<PandoraSong>();
            if (AvailableStations == null) AvailableStations = new List<PandoraStation>();

            _currentStation = null;
            PreviousSongs.Clear();
            AvailableStations.Clear();
            User = null;
        }

        protected void LoadMoreSongs() {
            List<PandoraSong> newSongs = new List<PandoraSong>();

            VerifyAndExecute(delegate {
                newSongs = pandora.GetSongs(User, CurrentStation);
            });

            // add our new songs to the playlist
            foreach (PandoraSong currSong in newSongs)
                playlist.Enqueue(currSong);
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
                if (User == null) throw new PandoraException("Username and/or password are no longer valid!");

                // and again, try the desired action
                logic();
            }            
        }

    }
}
