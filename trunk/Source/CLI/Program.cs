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

        bool needStatusUpdate = false;

        static void Main(string[] args) {

            Program p = new Program();

            if (p.Init())
                p.RunMainLoop();
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

            while (true) {
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
                        if (player.IsPlaying())
                            player.Stop();
                        else
                            player.Play();
                        break;
                    case 'n':
                        PlayNext();
                        break;
                }
            }
        }

        private void PrintStatus() {
            Console.Clear();
            Console.WriteLine("Station: {0}\n", musicBox.CurrentStation.Name);
            Console.WriteLine("Song:    {0}", musicBox.CurrentSong.Title);
            Console.WriteLine("Artist:  {0}", musicBox.CurrentSong.Artist);
            Console.WriteLine("Album:   {0}\n", musicBox.CurrentSong.Album);
            Console.Write(": ");
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
        */

        private void PlayNext() {
            player.Open(musicBox.GetNextSong());
            player.Play();
        }
    }
}
