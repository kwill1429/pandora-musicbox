using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using MediaPortal.Services;
using System.Reflection;
using PandoraMusicBox.MediaPortalPlugin.Properties;

namespace PandoraMusicBox.MediaPortalPlugin {
    internal class MusicBoxCore {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static bool loggerInitialized = false;

        public static MusicBoxCore Instance {
            get {
                if (_instance == null)
                    _instance = new MusicBoxCore();

                return _instance;
            }
        } private static MusicBoxCore _instance;

        public MusicBox MusicBox {
            get {
                if (_musicBox == null)
                    _musicBox = new MusicBox();

                return _musicBox;
            }
        } private MusicBox _musicBox;

        public MusicBoxSettings Settings {
            get;
            private set;
        }

        // Settings from Media Portal
        public static MediaPortal.Profile.Settings MediaPortalSettings {
            get {
                string settingsFile = MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml");
                MediaPortal.Profile.Settings mpSettings = new MediaPortal.Profile.Settings(settingsFile);
                return mpSettings;
            }
        }
        private MusicBoxCore() { }

        public void Initialize() {
            InitLogger();

            // log startup info
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            logger.Info("Pandora MusicBox " + ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision);
            logger.Info("Plugin Launched");

            Settings = new MusicBoxSettings();
            Settings.LoadSettings();

            ExtractResources();
        }

        public void Shutdown() {
            this.Settings.SaveSettings();

            logger.Info("Plugin Shutdown");
        }

        private void InitLogger() {
            if (loggerInitialized) return;
            loggerInitialized = true;

            string logFileName = "pandoramusicbox.log";
            string oldLogFileName = "pandoramusicBox.old.log";                

            // backup the current log file and clear for the new one
            try {
                FileInfo logFile = new FileInfo(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Log, logFileName));
                if (logFile.Exists) {
                    if (File.Exists(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Log, oldLogFileName)))
                        File.Delete(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Log, oldLogFileName));

                    logFile.CopyTo(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Log, oldLogFileName));
                    logFile.Delete();
                }
            }
            catch (Exception) { }

            // if no configuration exists go ahead and create one
            if (LogManager.Configuration == null) LogManager.Configuration = new LoggingConfiguration();

            // build the logging target for pandora music box logging
            FileTarget musicBoxLogTarget = new FileTarget();
            musicBoxLogTarget.Name = "pandora-musicbox";
            musicBoxLogTarget.FileName = MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Log, logFileName);
            musicBoxLogTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss} " +
                                "${level:fixedLength=true:padding=5} " +
                                "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " +
                                "${exception:format=tostring}";
            LogManager.Configuration.AddTarget("pandora-musicbox", musicBoxLogTarget);

            // Get current Log Level from MediaPortal 
            LogLevel logLevel;
            MediaPortal.Profile.Settings xmlreader = MediaPortalSettings;
            switch ((Level)xmlreader.GetValueAsInt("general", "loglevel", 0)) {
                case Level.Error:
                    logLevel = LogLevel.Error;
                    break;
                case Level.Warning:
                    logLevel = LogLevel.Warn;
                    break;
                case Level.Information:
                    logLevel = LogLevel.Info;
                    break;
                case Level.Debug:
                default:
                    logLevel = LogLevel.Debug;
                    break;
            }

            // always log in debug mode if the plugin was compiled in debug mode
            #if DEBUG
            logLevel = LogLevel.Debug;
            #endif

            // set the logging rules for pandora musicbox logging
            LoggingRule musicboxRule = new LoggingRule("PandoraMusicBox.*", logLevel, musicBoxLogTarget);
            LogManager.Configuration.LoggingRules.Add(musicboxRule);

            // force NLog to reload the configuration data
            LogManager.Configuration = LogManager.Configuration;
        }

        private void ExtractResources() {
            // define the location for and create the temp folder to contain our resources
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string dirName = Path.Combine(Path.GetTempPath(), "PandoraMusicBox");
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            // define the full paths to our files
            Settings.SadTrombone = Path.Combine(dirName, "sad-trombone.mp3");

            // Copy the resources to the temporary file
            try {
                if (!File.Exists(Settings.SadTrombone))
                    using (Stream outFile = File.Create(Settings.SadTrombone))
                        outFile.Write(Resources.SadTrombone, 0, Resources.SadTrombone.Length);
            }
            catch (Exception e) {
                logger.Warn("Failed saving the sad trombone: " + e.Message);
            }
        }
    }
}
