using System;
using System.Timers;
using Microsoft.Win32;

namespace Valutech.Station
{
    public delegate void StationConfigurationChangedEventHandler();

    public class StationConfiguration
    {
        /// <summary>
        /// Unique instance of the station configuration
        /// </summary>
        private static StationConfiguration instance;

        /// <summary>
        /// Timer to continuously check for changes in the configuration values
        /// </summary>
        private Timer timer = new Timer(1000);

        /// <summary>
        /// Indicates if the timer is started
        /// </summary>
        private bool started = false;

        #region Constants
        public static string STATIONS_SUMMARY_LOCATION = "StationsLocation";
        public static string STATION_PHYSICAL_LOCATION = "StationPhysicalLocation";
        public static string STATION_VNC_AVAILABLE = "VNC Available";
        public static string STATIONS_SUBKEY = @"SOFTWARE\Valutech\Updater\";
        #endregion

        /// <summary>
        /// Event triggered everytime a value in the configuration changed
        /// </summary>
        public event StationConfigurationChangedEventHandler StationConfigurationChanged;

        private string _stationsSummaryLocation = null;

        private string _stationPhysicalLocation = null;

        private bool _stationVNCAvailable = false;

        /// <summary>
        /// Constructor for the station configuration
        /// </summary>
        private StationConfiguration()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);            
        }

        /// <summary>
        /// Starts the station configuration values
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
            RegistryKey stationSubKey = OpenSubkey(StationConfiguration.STATIONS_SUBKEY);
            bool changed = false;            
            if (stationSubKey != null)
            {
                //Retrieve values
                object stationsSummaryKey = stationSubKey.GetValue(StationConfiguration.STATIONS_SUMMARY_LOCATION);
                object stationPhysicalLocationKey = stationSubKey.GetValue(StationConfiguration.STATION_PHYSICAL_LOCATION);
                object stationVNCAvailableKey = stationSubKey.GetValue(StationConfiguration.STATION_VNC_AVAILABLE);
                //Assign values                
                if (stationsSummaryKey != null)
                {
                    changed = (_stationsSummaryLocation != (string)stationsSummaryKey) ? true : changed;
                    this._stationsSummaryLocation = (string)stationsSummaryKey;
                }
                if (stationPhysicalLocationKey != null)
                {
                    changed = (_stationPhysicalLocation != (string)stationPhysicalLocationKey) ? true : changed;
                    this._stationPhysicalLocation = (string)stationPhysicalLocationKey;
                }
                if (stationVNCAvailableKey != null)
                {
                    changed = (_stationVNCAvailable != Convert.ToBoolean(stationVNCAvailableKey)) ? true : changed;
                    this._stationVNCAvailable = Convert.ToBoolean(stationVNCAvailableKey);
                }
            }
            if (changed)
            {
                if (StationConfigurationChanged != null) StationConfigurationChanged();
            }
        }

        /// <summary>
        /// Open subkey, if not found creates it
        /// </summary>
        /// <param name="subkeyText"></param>
        /// <returns></returns>
        private RegistryKey OpenSubkey(string subkeyText)
        {
            RegistryKey sk;
            try
            {
                sk = Registry.LocalMachine.OpenSubKey(subkeyText, true);
                if (sk == null) sk = Registry.LocalMachine.CreateSubKey(subkeyText, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch(System.Security.SecurityException)
            {                
                sk = Registry.LocalMachine.OpenSubKey(subkeyText);
                if (sk == null) sk = Registry.LocalMachine.CreateSubKey(subkeyText, RegistryKeyPermissionCheck.ReadSubTree);
            }
            return sk;
        }

        /// <summary>
        /// Gets the unique instance of the Station configuration class
        /// </summary>
        /// <returns></returns>
        public static StationConfiguration GetInstance()
        {
            if (instance == null)
            {
                instance = new StationConfiguration();
            }
            return instance;
        }

        #region Variable Getters

        /// <summary>
        /// Gets the current station summary location
        /// </summary>
        public string StationsSummaryLocation
        {
            get
            {
                if (!started) Start();
                return _stationsSummaryLocation;
            }
        }

        /// <summary>
        /// Gets or sets the current station physical location
        /// </summary>
        public string StationPhysicalLocation
        {
            get
            {
                if (!started) Start();
                return _stationPhysicalLocation;
            }
            set
            {
                try
                {
                    RegistryKey stationSubKey = OpenSubkey(StationConfiguration.STATIONS_SUBKEY);
                    if (stationSubKey != null) stationSubKey.SetValue(StationConfiguration.STATION_PHYSICAL_LOCATION, value);
                }
                catch
                {                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the current station vnc availability
        /// </summary>
        public bool StationVNCAvailable
        {
            get
            {
                if (!started) Start();
                return _stationVNCAvailable;
            }
            set
            {
                try
                {
                    RegistryKey stationSubKey = OpenSubkey(StationConfiguration.STATIONS_SUBKEY);
                    if (stationSubKey != null) stationSubKey.SetValue(StationConfiguration.STATION_VNC_AVAILABLE, value);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}