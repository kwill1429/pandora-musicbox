using System;
using System.Collections.Generic;
using Engine;
using PandoraMusicBox.CLI.Properties;
using PandoraMusicBox.Engine;
using PandoraMusicBox.Engine.Data;
using PandoraMusicBox.Engine.Encryption;
using DirectShowLib;
using System.Threading;

namespace PandoraMusicBox.CLI {
    class Program {
        MusicBox musicBox = new MusicBox();
        DirectShowPlayer player = new DirectShowPlayer();
        BlowfishCipher crypter = new BlowfishCipher(PandoraCryptKeys.In);

        Dictionary<int, PandoraStation> stationLookup = new Dictionary<int, PandoraStation>();

        bool needStatusUpdate = false;
        
        bool showHelp = false;
        bool showStations = false;

        static void Main(string[] args) {
            Program p = new Program();

            if (p.Init())
                p.RunMainLoop();

            Console.Clear();
            Console.Write("Pandora MusicBox is shutting down...\n");
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
                bool success = musicBox.Login(Settings.Default.Username, crypter.Decrypt(Settings.Default.EncryptedPassword));
                if (!success && !ManualLogin())
                    return false;
            }

            player.PlaybackEvent += new DirectShowPlayer.DirectShowEventHandler(player_PlaybackEvent);

            PlayNext();
            return true;
        }

        void player_PlaybackEvent(EventCode eventCode) {
            if (eventCode == EventCode.Complete) {
                needStatusUpdate = true;
                PlayNext();
            }
        }

        public void RunMainLoop() {
            ConsoleKeyInfo choice;

            do {
                PrintStatus();

                while (!Console.KeyAvailable) {
                    if (needStatusUpdate) {
                        PrintStatus();
                        needStatusUpdate = false;
                    }

                    Thread.Sleep(100);
                }

                choice = Console.ReadKey();
                switch (char.ToLower(choice.KeyChar)) {
                    case 'x':
                    case 'q':
                        return;
                    case ' ':
                        if (player.IsPlaying()) player.Stop();
                        else player.Play();
                        break;
                    case 'n':
                        PlayNext();
                        break;
                    case 's':
                        showStations = !showStations;
                        showHelp = false;
                        needStatusUpdate = true;
                        break;
                    case '?':
                    case 'h':
                        showHelp = !showHelp;
                        showStations = false;
                        needStatusUpdate = true;                        
                        break;
                }

                if (choice.Key == ConsoleKey.Escape) {
                    if (showHelp == true || showStations == true) {
                        showHelp = false;
                        showStations = false;
                        needStatusUpdate = true;
                    }
                    else {
                        return;
                    }
                }

                int stationIndex;
                if (int.TryParse(choice.KeyChar + "", out stationIndex)) {
                    if (stationLookup.ContainsKey(stationIndex) && stationLookup[stationIndex] != musicBox.CurrentStation) {
                        showStations = false;
                        musicBox.CurrentStation = stationLookup[stationIndex];
                        PlayNext();
                    }
                }

            } while (true);
        }

        private void PrintStatus() {
            Console.Clear();
            
            Console.WriteLine("Station: {0}\n", musicBox.CurrentStation.Name);
            Console.WriteLine("Song:    {0}", musicBox.CurrentSong.Title);
            Console.WriteLine("Artist:  {0}", musicBox.CurrentSong.Artist);
            Console.WriteLine("Album:   {0}\n", musicBox.CurrentSong.Album);

            if (showStations) PrintStations();
            if (showHelp) PrintHelp();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("press '?' for help");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(": ");
        }

        private void PrintStations() {
            Console.WriteLine("Available Stations:");

            int index = 0;
            foreach (PandoraStation currStation in musicBox.AvailableStations) {
                if (currStation.IsQuickMix) continue;

                index++;
                Console.WriteLine("{0}: {1}", index, currStation.Name);
                stationLookup[index] = currStation;
            }

            Console.WriteLine();
        }

        private void PrintHelp() {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("n     : Next Track");
            Console.WriteLine("s     : Show Station List");
            Console.WriteLine("SPACE : Play / Pause");
            Console.WriteLine("ESC   : Quit");
            Console.WriteLine();

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

                bool success = musicBox.Login(username, password);

                if (success) {
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


        /*
        private void ChooseStation() {
            
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
        */

        private void PlayNext() {
            player.Open(musicBox.GetNextSong());
            player.Play();
        }
    }
}
