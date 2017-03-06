using System;
using System.Timers;
using Microsoft.Win32;
using Valutech.Station;
using System.IO;

namespace Valutech.Configurations
{
    public delegate void ValutechUpdaterConfigurationChangedEventHandler();

    public class ValutechUpdaterConfiguration
    {
        public static string PROCESS_NAME = "ValutechUpdater";

        public static string APPLICATION_FRIENDLY_NAME = "Valutech Updater";

        private static string ONLINE_DIRECTORY = "ValutechUpdater";

        private static string LOCAL_INSTALLATION_FOLDER = "ValutechUpdater";

        private static string FILE_NAME = "ValutechUpdater.exe";

        private static string GP_UPDATE_FILE = "gpUpdate.bat";

        /// <summary>
        /// Unique instance of the valutech updater configuration
        /// </summary>
        private static ValutechUpdaterConfiguration instance;

        /// <summary>
        /// Timer to continuously check for changed in the configuration values
        /// </summary>
        private Timer timer = new Timer(1000);

        /// <summary>
        /// Indicates if the timer is started
        /// </summary>
        private bool started = false;

        #region Constants
        public static string UPDATES_LOCATION_KEY_NAME = "UpdatesLocation";
        public static string LOCAL_LOCATION_KEY_NAME = "LocalLocation";
        public static string SETUP_FILENAME_KEY_NAME = "SetupFilename";
        public static string VERSION_FILENAME_KEY_NAME = "VersionFilename";
        public static string TIME_BETWEEN_CHECKS_KEY_NAME = "TimeBetweenChecks";
        public static string LOG_LOCATION_KEY_NAME = "LogLocation";
        public static string LOCKED_KEY_NAME = "Locked";
        public static string UPDATES_SUBKEY = @"SOFTWARE\Valutech\Updater\";
        #endregion

        /// <summary>
        /// Event triggered everytime a value in the configuration changed
        /// </summary>
        public event ValutechUpdaterConfigurationChangedEventHandler ValutechUpdaterConfigurationChanged;

        private string _updatesLocation = null;
        private string _localLocation = null;
        private string _setupFileName = null;
        private string _versionFileName = null;
        private int _timeBetweenChecks = 0;
        private string _logsLocation = null;
        private bool _locked = false;

        /// <summary>
        /// Constructor for the valutech updater configuration
        /// </summary>
        private ValutechUpdaterConfiguration()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);            
        }

        /// <summary>
        /// Starts the reding of the valutech updater configuration values
        /// </summary>
        public void Start()
        {
            if (!started)
            {
                started = true;
                ReadValues();
                timer.Start();
            }
        }

        /// <summary>
        /// Read the values every minute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            ReadValues();
            timer.Enabled = true;
        }

        /// <summary>
        /// Reads the values from the registry and assign them to the variables and trigger the event if any of them changed
        /// </summary>
        private void ReadValues()
        {
            RegistryKey updatesSubKey = Registry.LocalMachine.OpenSubKey(ValutechUpdaterConfiguration.UPDATES_SUBKEY);
            bool changed = false;
            if (updatesSubKey == null) updatesSubKey = Registry.LocalMachine.CreateSubKey(ValutechUpdaterConfiguration.UPDATES_SUBKEY);
            if (updatesSubKey != null)
            {
                //Retrieve values
                object updatesLocationKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.UPDATES_LOCATION_KEY_NAME);
                object localLocationKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.LOCAL_LOCATION_KEY_NAME);
                object setupFileNameKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.SETUP_FILENAME_KEY_NAME);
                object versionFilenameKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.VERSION_FILENAME_KEY_NAME);
                object timeBetweenChecksKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.TIME_BETWEEN_CHECKS_KEY_NAME);
                object logsLocationKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.LOG_LOCATION_KEY_NAME);
                object lockedKey = updatesSubKey.GetValue(ValutechUpdaterConfiguration.LOCKED_KEY_NAME);
                //Assign values
                if (updatesLocationKey != null)
                {
                    changed = (_updatesLocation != (string)updatesLocationKey) ? true : changed;
                    this._updatesLocation = (string)updatesLocationKey;
                }
                if (localLocationKey != null)
                {
                    changed = (_localLocation != (string)localLocationKey) ? true : changed;
                    this._localLocation = (string)localLocationKey;
                }
                if (setupFileNameKey != null)
                {
                    changed = (_setupFileName != (string)setupFileNameKey)? true : changed;
                    this._setupFileName = (string)setupFileNameKey;
                }
                if (versionFilenameKey != null)
                {
                    changed = (_versionFileName != (string)versionFilenameKey) ? true : changed;
                    this._versionFileName = (string)versionFilenameKey;
                }
                if (timeBetweenChecksKey != null)
                {
                    changed = (_timeBetweenChecks != Convert.ToInt16(timeBetweenChecksKey)) ? true : changed;
                    this._timeBetweenChecks = Convert.ToInt16(timeBetweenChecksKey);
                }
                if (logsLocationKey != null && setupFileNameKey != null)
                {
                    changed = (_logsLocation != (string)logsLocationKey) ? true : changed;
                    this._logsLocation = (string)logsLocationKey;
                }
                if (lockedKey != null)
                {
                    changed = (_locked != Convert.ToBoolean(lockedKey)) ? true : changed;
                    this._locked = Convert.ToBoolean(lockedKey);
                }
            }
            if (changed)
            {
                if (ValutechUpdaterConfigurationChanged != null) ValutechUpdaterConfigurationChanged();
            }
        }

        /// <summary>
        /// Gets the unique instance of the Station configuration class
        /// </summary>
        /// <returns></returns>
        public static ValutechUpdaterConfiguration GetInstance()
        {
            if (instance == null)
            {
                instance = new ValutechUpdaterConfiguration();
            }
            return instance;
        }

        public static string GetOnlineSourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceGlobalLocation(facility), ONLINE_DIRECTORY);
        }

        public static string GetInstallationFolder()
        {
            return Path.Combine(StationsManagerStationConfiguration.GetLocalGlobalInstallationLocation(), LOCAL_INSTALLATION_FOLDER);
        }

        public static string GetApplicationPath()
        {
            return Path.Combine(GetInstallationFolder(), FILE_NAME);
        }

        public static string GetGPUpdatePath()
        {
            return Path.Combine(GetInstallationFolder(), GP_UPDATE_FILE);
        }

        public static string GetVersionPath()
        {
            return Path.Combine(GetInstallationFolder(), StationsManagerStationConfiguration.GetUpdatesVersionFile());
        }

        #region Variable Getters

        public string UpdatesLocation
        {
            get
            {
                if (!started) Start();
                return _updatesLocation;
            }
        }

        public string LocalLocation
        {
            get
            {
                if (!started) Start();
                return _localLocation;
            }
        }

        public string SetupFilename
        {
            get
            {
                if (!started) Start();
                return _setupFileName;
            }
        }

        public string VersionFilename
        {
            get
            {
                if (!started) Start();
                return _versionFileName;
            }
        }

        public int TimeBetweenChecks
        {
            get
            {
                if (!started) Start();
                return _timeBetweenChecks;
            }
        }

        public string LogsLocation
        {
            get
            {
                if (!started) Start();
                return _logsLocation;
            }
        }

        public bool Locked
        {
            get
            {
                if (!started) Start();
                return _locked;
            }
        }

        #endregion
    }
}