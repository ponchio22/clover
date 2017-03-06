using System;
using System.IO;
using Valutech.Station;
using System.Timers;
using Microsoft.Win32;

namespace Valutech.Configurations
{
    public delegate void WTMDisablerConfigurationChangedEventHandler();

    public enum WTMDisablerMode
    {
        PRODUCTION,
        RECEIVING_INSPECTION,
        AUDIT
    }

    public class WTMDisablerConfiguration
    {
        public static string PROCESS_NAME = "igfawd";

        public static string APPLICATION_FRIENDLY_NAME = "WTM Disabler";

        private static string ONLINE_DIRECTORY = "igfawd";

        private static string LOCAL_INSTALLATION_FOLDER = "WTMDisabler";

        private static string FILE_NAME = "igfawd.exe";

        private static string INSTALLER_FILE_NAME = "igfawdInstaller.bat";

        public static string MODE_KEY_NAME = "Mode";

        public static string WTMDISABLER_SUBKEY = @"SOFTWARE\Valutech\WTMDisabler\";

        private int _mode = -1;

        /// <summary>
        /// Event triggered everytime a value in the configuration changed
        /// </summary>
        public event WTMDisablerConfigurationChangedEventHandler ConfigurationChanged;

        private static WTMDisablerConfiguration instance;

        /// <summary>
        /// Timer to continuously check for changed in the configuration values
        /// </summary>
        private Timer timer = new Timer(1000);

        /// <summary>
        /// Indicates if the timer is started
        /// </summary>
        private bool started = false;

        /// <summary>
        /// Constructor for the valutech updater configuration
        /// </summary>
        private WTMDisablerConfiguration()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);            
        }

        /// <summary>
        /// Gets the unique instance of the Station configuration class
        /// </summary>
        /// <returns></returns>
        public static WTMDisablerConfiguration GetInstance()
        {
            if (instance == null)
            {
                instance = new WTMDisablerConfiguration();
            }
            return instance;
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
            try
            {
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey(WTMDisablerConfiguration.WTMDISABLER_SUBKEY);
                bool changed = false;
                if (subKey == null) subKey = Registry.LocalMachine.CreateSubKey(WTMDisablerConfiguration.WTMDISABLER_SUBKEY);
                if (subKey != null)
                {
                    //Retrieve values
                    object modeKey = subKey.GetValue(WTMDisablerConfiguration.MODE_KEY_NAME);
                    //Assign values
                    if (modeKey != null)
                    {
                        changed = (_mode != (int)modeKey) ? true : changed;
                        this._mode = (int)modeKey;
                    }                    
                }
                if (changed)
                {
                    if (ConfigurationChanged != null) ConfigurationChanged();
                }
            }
            catch
            {
            }
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

        public static string GetInstallerPath()
        {
            return Path.Combine(GetInstallationFolder(), INSTALLER_FILE_NAME);
        }

        public int Mode
        {
            get
            {
                if (!started) Start();
                return this._mode;
            }
        }
    }
}