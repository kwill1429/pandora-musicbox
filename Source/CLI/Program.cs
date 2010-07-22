using System;
using System.Collections.Generic;
using Engine;
using PandoraMusicBox.CLI.Properties;
using PandoraMusicBox.Engine;
using PandoraMusicBox.Engine.Data;
using PandoraMusicBox.Engine.Encryption;

namespace PandoraMusicBox.CLI {
    class Program {
        PandoraUser user;
        List<PandoraStation> stations;

        MusicBoxCore musicBox = new MusicBoxCore();
        BlowfishCipher crypter = new BlowfishCipher(PandoraCryptKeys.In);


        static void Main(string[] args) {

            Program p = new Program();
            
            if (p.Init()) {
                p.ChooseStation();
            }
        }

        private bool Init() {
            CheckForUpgrade();

            // if we have no login credentials, prompt the user
            if (Settings.Default.Username == string.Empty) {
                if (!ManualLogin()) 
                    return false;
            }

            // otherwise try to login, and if failed prompt the user
            else {
                user = musicBox.AuthenticateListener(Settings.Default.Username, crypter.Decrypt(Settings.Default.EncryptedPassword));
                if (user == null && !ManualLogin())
                    return false;
            }

            // load all stations
            stations = musicBox.GetStations(user);

            return true;
        }

        private void CheckForUpgrade() {
            if (Settings.Default.UpgradeRequired) {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }

        private bool ManualLogin() {
            bool retry = true;

            while (retry) {
                Console.Clear();
                Console.Write("User Name: ");
                string username = Console.ReadLine();

                Console.Write("Password:  ");
                string password = Console.ReadLine();

                user = musicBox.AuthenticateListener(username, password);

                if (user != null) {
                    Properties.Settings.Default.Username = username;
                    Properties.Settings.Default.EncryptedPassword = crypter.Encrypt(password);
                    Properties.Settings.Default.Save();
                    return true;
                }
                else {
                    Console.Clear();
                    Console.Write("Invalid user name or password. Retry? (y/n)");
                    ConsoleKeyInfo keyPress = Console.ReadKey();
                    
                    if (keyPress.KeyChar != 'y' && keyPress.KeyChar != 'Y')
                        retry = false;                   
                }
            }

            return false;
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
            DirectShowPlayer player = new DirectShowPlayer();
            
            Console.Clear();
            Console.WriteLine("Station: {0}\n", station.Name);

            List<PandoraSong> songs = musicBox.GetSongs(user, station);
            
            foreach(PandoraSong currSong in songs) {
                Console.WriteLine("Now Playing: '{0}' by {1}", currSong.Title, currSong.Artist);
                player.Open(currSong);
                player.Play();

                Console.ReadKey();
            }

            

            Console.ReadKey();
        }
    }
}
