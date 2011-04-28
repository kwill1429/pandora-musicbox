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
        const ConsoleColor DEFAULT_BACKGROUND_COLOR = ConsoleColor.Black;

        bool needUIUpdate = false;
        bool modalWindowDisplayed = false;
        bool showHelp = false;
        Thread waitIconThread;
        object printTextLock = new object();

        static void Main(string[] args) {
            Program p = new Program();

            try {
                if (p.Init())
                    p.RunMainLoop();
            }
            catch (PandoraException pe) {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
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
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Very sorry, but something has gone horribly wrong!\n");
                Console.WriteLine("{0}", e.ToString());
                Console.ReadKey();
            }

            Console.Clear();
            Console.Write("Pandora MusicBox is shutting down...\n");
        }

        private bool Init() {
            Console.CursorVisible = false;
            Console.WindowWidth = 80;
            Console.WindowHeight = 25;
            Console.BufferWidth = 80;
            Console.BufferHeight = 25;
            Console.WindowLeft = 0;
            Console.WindowTop = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

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
                try {
                    bool success = musicBox.Login(Settings.Default.Username, crypter.Decrypt(Settings.Default.EncryptedPassword));
                    if (!success && !ManualLogin())
                        return false;
                }
                catch (PandoraException) {
                    if (!ManualLogin())
                        return false;
                }
            }
            ShowWaitIcon(true);

            player.PlaybackEvent += new DirectShowPlayer.DirectShowEventHandler(player_PlaybackEvent);

            if (Settings.Default.LastStationId != "") {
                foreach (PandoraStation station in musicBox.AvailableStations) {
                    if (station.IsQuickMix) continue;

                    if (station.Id == Settings.Default.LastStationId) {
                        musicBox.CurrentStation = station;
                    }
                }
            }

            player.Volume = Settings.Default.Volume;
            PlayNext(false);
            ShowWaitIcon(false);
            return true;
        }

        void player_PlaybackEvent(EventCode eventCode) {
            if (eventCode == EventCode.Complete) {
                PlayNext(false);
            }
        }

        public void RunMainLoop() {
            ConsoleKeyInfo choice;
            PrintUI();

            do {

                while (!Console.KeyAvailable) {
                    if (needUIUpdate) {
                        PrintUI();
                        needUIUpdate = false;
                    }
                    PrintSongPosition();
                    Thread.Sleep(100);
                }

                choice = Console.ReadKey(true);
                switch (char.ToLower(choice.KeyChar)) {
                    case 'x':
                    case 'q':
                        return;
                    case ' ':
                        if (player.IsPlaying()) player.Stop();
                        else player.Play();
                        break;
                    case 'n':
                        PlayNext(true);
                        break;
                    case 's':
                        PandoraStation newStation = ShowStationChooser();
                        if (newStation != null && newStation != musicBox.CurrentStation) {
                            ShowWaitIcon(true);
                            musicBox.CurrentStation = newStation;
                            Settings.Default.LastStationId = musicBox.CurrentStation.Id;
                            Settings.Default.Save();
                            PlayNext(false);
                            ShowWaitIcon(false);
                        }
                        break;
                    case '?':
                    case 'h':
                        showHelp = !showHelp;
                        modalWindowDisplayed = showHelp;
                        needUIUpdate = true;
                        break;
                    case '+':
                        musicBox.RateSong(musicBox.CurrentSong, PandoraRating.Love);
                        needUIUpdate = true;
                        break;
                    case '-':
                        musicBox.RateSong(musicBox.CurrentSong, PandoraRating.Hate);
                        PlayNext(true);
                        break;
                    case 'b':
                        musicBox.TemporarilyBanSong(musicBox.CurrentSong);
                        PlayNext(true);
                        break;
                    case 'p':
                        PrintText("                         ", 0, 7);
                        Settings.Default.DisplayPosition = !Settings.Default.DisplayPosition;
                        Settings.Default.Save();
                        break;
                }

                if (choice.Key == ConsoleKey.Escape) {
                    if (showHelp) {
                        showHelp = false;
                        modalWindowDisplayed = false;
                        needUIUpdate = true;
                    }
                    else {
                        return;
                    }
                }

                if (choice.Key == ConsoleKey.UpArrow) {
                    player.Volume += 0.01;
                    PrintVolume();
                    Settings.Default.Volume = player.Volume;
                    Settings.Default.Save();
                }

                if (choice.Key == ConsoleKey.DownArrow) {
                    player.Volume -= 0.01;
                    PrintVolume();
                    Settings.Default.Volume = player.Volume;
                    Settings.Default.Save();
                }

                if (choice.Key == ConsoleKey.RightArrow) {
                    PlayNext(true);
                }

            } while (true);
        }


        private void PrintUI() {
            if (!modalWindowDisplayed) {
                PrintSongDetails();
                PrintVolume();
            }
            if (showHelp) PrintHelp();
        }


        private void PrintSongDetails() {
            Console.Clear();
            Console.SetCursorPosition(30, 0);
            PrintText("Pandora MusicBox CLI", 30, 0, ConsoleColor.White);
            PrintText("Station: ", 0, 1, ConsoleColor.DarkGray);
            PrintText("Song:    ", 0, 3, ConsoleColor.DarkGray);
            PrintText("Artist:  ", 0, 4, ConsoleColor.DarkGray);
            PrintText("Album:   ", 0, 5, ConsoleColor.DarkGray);


            PrintText(musicBox.CurrentStation.Name, 9, 1);
            ConsoleColor songColor;
            if (musicBox.CurrentSong.Rating == PandoraRating.Love)
                songColor = ConsoleColor.Green;
            else
                songColor = ConsoleColor.Gray;

            PrintText(musicBox.CurrentSong.Title, 9, 3, songColor);
            PrintText(musicBox.CurrentSong.Artist, 9, 4);
            PrintText(musicBox.CurrentSong.Album, 9, 5);

            PrintText("press 'h' for help", 0, Console.WindowHeight - 1, ConsoleColor.DarkGray);
        }

        private void PrintSongPosition() {
            if (modalWindowDisplayed) return;
            if (!Settings.Default.DisplayPosition) return;

            TimeSpan position = new TimeSpan();
            TimeSpan length = new TimeSpan();

            try {
                position = TimeSpan.FromMilliseconds(player.Position);
                length = player.GetLoadedAudioFile().Length;
            }
            catch (NullReferenceException) {
                // player doesn't have an audio file yet.
                // default time values should be are fine
            }
            string time =
                String.Format("{0:00}:{1:00} / {2:00}:{3:00}"
                    , position.Minutes, position.Seconds
                    , length.Minutes, length.Seconds
                    );

            if (!player.IsPlaying())
                time += " (paused)";

            PrintText(time.PadRight(Console.WindowWidth), 0, 7);
        }

        private void PrintVolume() {
            if (modalWindowDisplayed) return;

            PrintText("Vol:", Console.WindowWidth - 8, 0, ConsoleColor.DarkGray);
            string volume = ((int)(player.Volume * 100)).ToString().PadLeft(3);
            PrintText(volume, Console.WindowWidth - 3, 0);
        }

        private PandoraStation ShowStationChooser() {
            int childWidth = 54;
            int innerWidth = childWidth - 4;
            int childHeight = Console.WindowHeight - 5;
            int childX = (Console.WindowWidth - childWidth) / 2;
            int childY = 2;
            int selectedIndex = 0;
            int scrollTop = 0;
            int lineCount = childHeight - 4;
            bool needStationMenuUpdate = true;

            modalWindowDisplayed = true;
            try {
                List<PandoraStation> stations = new List<PandoraStation>();
                foreach (PandoraStation station in musicBox.AvailableStations) {
                    if (station.IsQuickMix) continue;
                    stations.Add(station);

                    if (station == musicBox.CurrentStation)
                        selectedIndex = stations.Count - 1;
                }

                ConsoleKeyInfo key;

                do {
                    if (needStationMenuUpdate) {
                        // print window border
                        PrintWindowBorder(childX, childY, childWidth, childHeight, ConsoleColor.DarkCyan, "Available Stations");

                        string hint = string.Format(" {0}/{1} ", selectedIndex + 1, stations.Count);
                        PrintText(hint, childX + 2, childY + childHeight, ConsoleColor.DarkCyan);

                        // right align
                        PrintText("Select a new station, or [ESC]", childX + childWidth - 32, childY + childHeight - 1, ConsoleColor.DarkGray);

                        // determine if the selected item is visible
                        if (selectedIndex < scrollTop)
                            scrollTop = selectedIndex;

                        while (selectedIndex >= scrollTop + lineCount) {
                            scrollTop++;
                        }

                        // print menu items
                        for (int line = 0; line < lineCount; line++) {
                            if (line + scrollTop >= stations.Count) break;
                            PandoraStation currStation = stations[line + scrollTop];
                            int songNumber = line + scrollTop + 1;
                            string printLine = String.Format("{0}: {1}", songNumber.ToString().PadLeft(2), currStation.Name);
                            if (printLine.Length > innerWidth)
                                printLine = printLine.Substring(0, innerWidth);

                            ConsoleColor backgroundColor = ConsoleColor.Black;
                            if (line + scrollTop == selectedIndex)
                                backgroundColor = ConsoleColor.DarkCyan;

                            PrintText(printLine.PadRight(innerWidth), childX + 2, childY + 2 + line, ConsoleColor.Gray, backgroundColor);

                        }
                        needStationMenuUpdate = false;
                    }

                    key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape) {
                        modalWindowDisplayed = false;
                        needUIUpdate = true;
                        return null;
                    }

                    if (key.Key == ConsoleKey.DownArrow) {
                        selectedIndex++;
                        if (selectedIndex > stations.Count - 1)
                            selectedIndex = stations.Count - 1;
                        else
                            needStationMenuUpdate = true;
                    }

                    if (key.Key == ConsoleKey.UpArrow) {
                        selectedIndex--;
                        if (selectedIndex < 0)
                            selectedIndex = 0;
                        else
                            needStationMenuUpdate = true;
                    }

                } while (key.Key != ConsoleKey.Enter);

                return stations[selectedIndex];
            }
            finally {
                modalWindowDisplayed = false;
            }
        }

        private void PrintHelp() {
            int childWidth = 54;
            int innerWidth = childWidth - 4;
            int childHeight = Console.WindowHeight - 5;
            int childX = (Console.WindowWidth - childWidth) / 2;
            int childY = 2;


            string helpWindow = "";
            helpWindow += "Available Commands:\n";
            helpWindow += "\n";
            helpWindow += "SPACE : Play / Pause\n";
            helpWindow += "RIGHT : Skip to Next Song\n";
            helpWindow += "UP    : Increase Volume\n";
            helpWindow += "DOWN  : Decrease Volume\n";
            helpWindow += "\n";
            helpWindow += "s     : Show Station List\n";
            helpWindow += "\n";
            helpWindow += "+     : I Like This Song\n";
            helpWindow += "-     : I Don't Like This Song\n";
            helpWindow += "b     : Temporarily Ban This Song (One Month)\n";
            helpWindow += "\n";
            helpWindow += "p     : Toggle Progress Indicators\n";
            helpWindow += "\n";
            helpWindow += "ESC   : Quit / Back\n";

            string[] lines = helpWindow.Split('\n');

            // print window border
            PrintWindowBorder(childX, childY, childWidth, childHeight, ConsoleColor.DarkCyan, "Help");

            // print text
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            int index = 0;
            foreach (string line in lines) {
                index++;
                string printLine = line;
                if (printLine.Length > innerWidth)
                    printLine = printLine.Substring(0, innerWidth);
                PrintText(printLine, childX + 2, childY + 1 + index);
            }

            // right aligned
            PrintText("[ESC] to close help", childX + childWidth - 21, childY + childHeight - 1, ConsoleColor.DarkGray);
        }

        private void PrintWindowBorder(int x, int y, int width, int height, ConsoleColor foregroundColor, string title) {
            string textBuffer;

            // top border
            textBuffer = string.Format("╔═ {0} {1}╗", title, new String('═', width - title.Length - 5));
            PrintText(textBuffer, x, y, foregroundColor);

            // sides
            textBuffer = string.Format("║{0}║", new string(' ', width - 2));
            for (int i = 1; i < height; i++) {
                PrintText(textBuffer, x, y + i, foregroundColor);
            }

            // bottom border
            textBuffer = string.Format("╚{0}╝", new String('═', width - 2));
            PrintText(textBuffer, x, y + height, foregroundColor);
        }

        private void PrintText(string text, int x, int y, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = DEFAULT_BACKGROUND_COLOR) {
            lock (printTextLock) {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.SetCursorPosition(x, y);
                Console.Write(text);
                Console.BackgroundColor = DEFAULT_BACKGROUND_COLOR;
            }
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

        private void PlayNext(bool isSkip) {
            if (isSkip && !musicBox.CanSkip())
                return;

            ShowWaitIcon(true);

            player.Open(musicBox.GetNextSong(isSkip));
            player.Play();
            PrintUI();

            ShowWaitIcon(false);
            needUIUpdate = true;
        }

        private void ShowWaitIcon(bool display) {
            if (display && Settings.Default.DisplayPosition)
            {
			    // do we need some type of a lock here?
                if (waitIconThread == null || !waitIconThread.IsAlive) {
                    // show wait icon
                    ThreadStart actions = delegate {
                        int i = 0;
                        while (true) {
                            char[] chars = new char[] { '\\', '|', '/', '-' };
                            i++;
                            if (i > chars.Length - 1)
                                i = 0;
                            PrintText(chars[i].ToString(), Console.WindowWidth - 10, 0, ConsoleColor.Yellow);
                            Thread.Sleep(100);
                        }
                    };
                    waitIconThread = new Thread(actions);
                    waitIconThread.IsBackground = true;
                    waitIconThread.Start();
                }
            }
            else {
                if (waitIconThread != null)
                    waitIconThread.Abort();
                PrintText(" ", Console.WindowWidth - 10, 0, ConsoleColor.Yellow);
            }
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
