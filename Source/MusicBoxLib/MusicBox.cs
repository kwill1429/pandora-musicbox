using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    public class MusicBox {

        protected PandoraIO pandora = new PandoraIO();
        protected Queue<PandoraSong> playlist = new Queue<PandoraSong>();

        public PandoraUser User {
            get;
            protected set;
        }

        public PandoraStation CurrentStation {
            get { return _currentStation; }
            set {
                if (AvailableStations.Contains(value))
                    _currentStation = value;
            }
        } protected PandoraStation _currentStation;

        public PandoraSong CurrentSong {
            get;
            protected set;
        }

        public List<PandoraSong> PreviousSongs {
            get;
            protected set;
        }

        public List<PandoraStation> AvailableStations {
            get;
            protected set;
        }


        public bool Login(string username, string password) {
            Clear();

            User = pandora.AuthenticateListener(username, password);
            if (User != null) {
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

        public void Logout() {
            Clear();
        }

        public PandoraSong GetNextSong() {
            while (PreviousSongs.Count > 4) 
                PreviousSongs.RemoveAt(0);
            PreviousSongs.Add(CurrentSong);

            if (playlist.Count < 3)
                LoadMoreSongs();

            CurrentSong = playlist.Dequeue();            
            return CurrentSong;
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
            foreach (PandoraSong currSong in pandora.GetSongs(User, CurrentStation))
                playlist.Enqueue(currSong);
        }

    }
}
