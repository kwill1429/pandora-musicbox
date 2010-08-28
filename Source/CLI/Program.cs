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
        int newStationIndex = 0;

        static void Main(string[] args) {
            Program p = new Program();

            try {                
                if (p.Init())
                    p.RunMainLoop();
            }
            catch (PandoraException pe) {
                Console.Clear();
                Console.WriteLine("Very sorry, but something has gone horribly wrong!\n");
                Console.WriteLine("AppData: {0}", System.Windows.Forms.Application.LocalUserAppDataPath);
                if (pe.ErrorCode != ErrorCodeEnum.UNKNOWN) Console.Write("{0}: ", pe.ErrorCode.ToString());
                Console.WriteLine(pe.Message);

                if (pe.XmlString != null) Console.WriteLine("\nXML Data:\n {0}", pe.XmlString);

                Console.WriteLine("\n{0}\n", pe.StackTrace);

                if (pe.InnerException != null) {
                    Console.WriteLine(pe.InnerException.ToString());
                }
                Console.ReadKey();
            }
            catch (Exception e) {
                Console.Clear();
                Console.WriteLine("Very sorry, but something has gone horribly wrong!\n");
                Console.WriteLine("{0}", e.ToString());
                Console.ReadKey();
            }

            Console.Clear();
            Console.Write("Pandora MusicBox is shutting down...\n");
        }

        private bool Init() {
            CheckForSettingsUpgrade();

            // if we have no login credentials, prompt the user
            if (Settings.Default.Username == string.Empty) {
                if (!ManualLogin()) 
                    return false;
            }

            // otherwise try to login, and if failed prompt the user
            else {
                Console.Clear();
                Console.Write("Attempting to login with saved credentials...\n");

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
                PlayNext();
                needStatusUpdate = true;
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
                        newStationIndex = 0;
                        showHelp = false;
                        needStatusUpdate = true;
                        break;
                    case '?':
                    case 'h':
                        showHelp = !showHelp;
                        showStations = false;
                        needStatusUpdate = true;                        
                        break;
                    case '+':
                        musicBox.RateSong(musicBox.CurrentSong, PandoraRating.Love);
                        needStatusUpdate = true;
                        break;
                    case '-':
                        musicBox.RateSong(musicBox.CurrentSong, PandoraRating.Hate);
                        PlayNext();
                        break;
                    case 'b':
                        musicBox.TemporarilyBanSong(musicBox.CurrentSong);
                        PlayNext();
                        break;
                }

                if (choice.Key == ConsoleKey.Escape) {
                    if (showHelp == true || showStations == true) {
                        showHelp = false;
                        showStations = false;
                        needStatusUpdate = true;
                        newStationIndex = 0;
                    }
                    else {
                        return;
                    }
                } 

                if (choice.Key == ConsoleKey.UpArrow) {
                    player.Volume += 0.01;
                }

                if (choice.Key == ConsoleKey.DownArrow) {
                    player.Volume -= 0.01;
                }

                if (choice.Key == ConsoleKey.RightArrow) {
                    PlayNext();
                }


                if (showStations) {
                    int stationInput;
                    if (int.TryParse(choice.KeyChar + "", out stationInput)) {
                        newStationIndex = int.Parse(newStationIndex.ToString() + stationInput.ToString());
                    }
                }

                if (choice.Key == ConsoleKey.Enter) {
                    if (showStations && newStationIndex > 0) {
                        if (stationLookup.ContainsKey(newStationIndex) && stationLookup[newStationIndex] != musicBox.CurrentStation) {
                            musicBox.CurrentStation = stationLookup[newStationIndex];
                            PlayNext();
                        }
                        showStations = false;
                        newStationIndex = 0;
                    }
                }

            } while (true);
        }

        private void PrintStatus() {
            Console.Clear();
            ConsoleColor origColor = Console.ForegroundColor;
            
            Console.WriteLine("Station: {0}\n", musicBox.CurrentStation.Name);
            Console.Write("Song:    ");
            if (musicBox.CurrentSong.Rating == PandoraRating.Love) {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write("{0}\n", musicBox.CurrentSong.Title);
            Console.ForegroundColor = origColor;
            
            Console.WriteLine("Artist:  {0}", musicBox.CurrentSong.Artist);
            Console.WriteLine("Album:   {0}\n", musicBox.CurrentSong.Album);

            if (showStations) PrintStations();
            if (showHelp) PrintHelp();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("press '?' for help");
            Console.ForegroundColor = ConsoleColor.Gray;
            if (showStations)
                Console.Write("Station: " + (newStationIndex > 0 ? newStationIndex.ToString() : ""));
            else
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
            Console.WriteLine(); 
            Console.WriteLine("SPACE : Play / Pause");
            Console.WriteLine("RIGHT : Skip to Next Song");
            Console.WriteLine("UP    : Increase Volume");
            Console.WriteLine("DOWN  : Decrease Volume");
            Console.WriteLine();
            Console.WriteLine("s     : Show Station List");
            Console.WriteLine();
            Console.WriteLine("+     : I Like This Song");
            Console.WriteLine("-     : I Don't Like This Song");
            Console.WriteLine("b     : Temporarily Ban This Song (One Month)");
            Console.WriteLine();
            Console.WriteLine("ESC   : Quit / Back");
            Console.WriteLine();

        }

        private void CheckForSettingsUpgrade() {
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
                string password = ReadPassword();

                Console.Clear();
                Console.Write("Attempting to login...\n");

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

        private void PlayNext() {
            player.Open(musicBox.GetNextSong(false));
            player.Play();
        }

        private string ReadPassword() {
            ConsoleKeyInfo info;
            string password = "";

            info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter) {
                if (info.Key != ConsoleKey.Backspace) {
                    password += info.KeyChar;
                    Console.Write('·');
                }
                else if (info.Key == ConsoleKey.Backspace && !string.IsNullOrEmpty(password)) {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }

                info = Console.ReadKey(true);
            }

            return password;
        }
    }
}
