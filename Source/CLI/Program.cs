using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Encryption;
using PandoraMusicBox.Engine;
using PandoraMusicBox.Engine.Data;

namespace CLI {
    class Program {
        PandoraUser user;
        List<PandoraStation> stations;

        MusicBoxCore musicBox = new MusicBoxCore();

        static void Main(string[] args) {

            Program p = new Program();
            
            if (p.Login()) {
                p.ChooseStation();
            }
        }

        private bool Login() {
            string username = "user@name.com";
            string pw = "password";

            user = musicBox.AuthenticateListener(username, pw);
            if (user != null) {
                stations = musicBox.GetStations(user);
                return true;
            }
            else {
                Console.WriteLine("invalid username or password.");
                
                #if DEBUG
                Console.ReadKey();
                #endif

                return false;
            }
        }

        private void ChooseStation() {
            var stationLookup = new Dictionary<int, PandoraStation>();
            string choice = "";


            while (choice.ToLower() != "x") {
                Console.Clear();
                Console.WriteLine("\n\nAvailable Stations [{0}]:", user.Name);

                int index = 0;
                foreach (PandoraStation currStation in stations) {
                    if (currStation.IsQuickMix) continue;

                    index++;
                    Console.WriteLine("{0:00}: {1}", index, currStation.Name);
                    stationLookup[index] = currStation;

                }

                Console.WriteLine("\nx : Exit Program\n\n");
                Console.Write(": ");
                choice = Console.ReadLine();

                int stationIndex;
                if (int.TryParse(choice, out stationIndex)) {
                    if (stationLookup.ContainsKey(stationIndex))
                        Play(stationLookup[stationIndex]);                
                }
            }
        }

        private void Play(PandoraStation station) {
            Console.Clear();
            Console.WriteLine("Station: {0}\n", station.Name);

            List<PandoraSong> songs = musicBox.GetSongs(user, station);
            foreach (PandoraSong currSong in songs) ;
            Console.WriteLine("Now Playing: {0}", "???");

            

            Console.ReadKey();
        }
    }
}
