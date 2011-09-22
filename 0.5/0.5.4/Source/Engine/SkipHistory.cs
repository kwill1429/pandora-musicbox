using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Data;

namespace PandoraMusicBox.Engine {
    public class SkipHistory {
        public static readonly int? BasicDefaultSkipsPerHour = 6;
        public static readonly int? BasicDefaultSkipsPerDay = 12;

        public static readonly int? PremiumDefaultSkipsPerHour = 6;
        public static readonly int? PremiumDefaultSkipsPerDay = null;
      
        protected Dictionary<string, Queue<DateTime>> stationSkipHistory;
        protected Queue<DateTime> globalSkipHistory;


        public PandoraUser User {
            get { return _user; }
            
            internal set {
                if (value == null || value.AccountType == AccountType.BASIC) {
                    AllowedStationsSkipsPerHour = BasicDefaultSkipsPerHour;
                    AllowedSkipsPerDay = BasicDefaultSkipsPerDay;
                }
                else {
                    AllowedStationsSkipsPerHour = PremiumDefaultSkipsPerHour;
                    AllowedSkipsPerDay = PremiumDefaultSkipsPerDay; 
                }

                _user = value;
            }
        } protected PandoraUser _user;

        public int? AllowedStationsSkipsPerHour {
            get { return _allowedStationsSkipsPerHour; }
            internal set { _allowedStationsSkipsPerHour = value; }            
        } protected int? _allowedStationsSkipsPerHour = 6;

        public int? AllowedSkipsPerDay {
            get { return _allowedSkipsPerDay; }
            internal set { _allowedSkipsPerDay = value; }
        } protected int? _allowedSkipsPerDay = 12;


        internal SkipHistory(PandoraUser user)
            : this() {

            _user = user;
        }

        internal SkipHistory() {
            stationSkipHistory = new Dictionary<string, Queue<DateTime>>();
            globalSkipHistory = new Queue<DateTime>();
        }


        public void Skip(PandoraStation station) {
            if (!CanSkip(station))
                throw new PandoraException("User is not currently allowed to skip tracks.");

            // log the current time as a skip event
            stationSkipHistory[station.Id].Enqueue(DateTime.Now);
            globalSkipHistory.Enqueue(DateTime.Now);
        }

        public bool CanSkip(PandoraStation station) {
            // remove any skip history records older than an hour
            foreach (Queue<DateTime> currHistory in stationSkipHistory.Values) {
                while (currHistory.Count > 0 && DateTime.Now - currHistory.Peek() > new TimeSpan(1, 0, 0))
                    currHistory.Dequeue();
            }

            // remove any daily skip history older than a day
            while (globalSkipHistory.Count > 0 && DateTime.Now - globalSkipHistory.Peek() > new TimeSpan(24, 0, 0))
                globalSkipHistory.Dequeue();

            // if the current station has no skip history, add it
            if (!stationSkipHistory.ContainsKey(station.Id))
                stationSkipHistory.Add(station.Id, new Queue<DateTime>());

            // if we are allowed to skip, record the current time and finish
            if (IsGlobalSkipAllowed() && IsStationSkipAllowed(station)) {
                return true;
            }

            // too bad, no skip for you!
            return false;
        }

        private bool IsStationSkipAllowed(PandoraStation station) {
            if (AllowedStationsSkipsPerHour == null)
                return true;

            if (stationSkipHistory[station.Id].Count < AllowedStationsSkipsPerHour)
                return true;

            return false;
        }

        private bool IsGlobalSkipAllowed() {
            if (AllowedSkipsPerDay == null)
                return true;

            if (globalSkipHistory.Count < AllowedSkipsPerDay)
                return true;

            return false;
        }
    }
}
