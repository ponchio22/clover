using System;
using System.IO;

namespace Valutech.Station
{
    public class StationsManagerStationConfiguration
    {
        public enum Facilities
        {
            Facility1
        }
        
        #region Global Constants

        //Facility 1
        private const string UPDATES_GLOBAL_LOCATION_F1 = @"\\file-server\Files\49_ENG_TOOLS\Updates\";
        private const string UPDATED_SOURCE_GLOBAL_LOCATION_F1 = @"\\file-server\Files\49_ENG_TOOLS\UpdatedSource\";
        private const string STATIONS_SUMMARY_GLOBAL_LOCATION_F1 = @"\\datafeed-new\Valutech\";
        private const string UPDATED_SOURCE_IT_GLOBAL_LOCATION_F1 = @"\\file-server\ITSupport\IT Apps\";
        private const string FILE_SYNC_UPDATED_SOURCE_GLOBAL_LOCATION_F1 = @"\\file-server\files\49_ENG_TOOLS\";
        private const string FILE_SYNC_UPDATED_SYNC_DATA_GLOBAL_LOCATION_F1 = @"\\file-server\files\49_ENG_TOOLS\";
        
        //Common values        
        private const string UPDATES_VERSION_FILENAME = "Version.xml";
        private const string UPDATES_SETUP_FILENAME = "Setup.exe";
        private const string LOCAL_GLOBAL_INSTALLATION_LOCATION = @"VALUTECH";
        private const int TIME_BETWEEN_CHECKS = 30; //Seconds

        #endregion

        private const string STATION_SUMMARY_LOCATION = @"ET\SMStations\";
        private const string UPDATES_LOCATION = @"StationsManager\";
        private const string LOCAL_INSTALLATION_FOLDER = @"StationsManager\";
        public static string START_UP_REGISTRY_ROUTE = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        #region Global locations

        public static string GetFileSyncUpdaterSourceGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = FILE_SYNC_UPDATED_SOURCE_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetFileSyncUpdatedSyncDataGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = FILE_SYNC_UPDATED_SYNC_DATA_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetUpdatesGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = UPDATES_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetUpdatedSourceITGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = UPDATED_SOURCE_IT_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetUpdatedSourceGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = UPDATED_SOURCE_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetStationsSummaryGlobalLocation(Facilities facility)
        {
            string location = string.Empty;
            if (facility == Facilities.Facility1)
            {
                location = STATIONS_SUMMARY_GLOBAL_LOCATION_F1;
            }
            return location;
        }

        public static string GetLocalGlobalInstallationLocation()
        {
            return Path.Combine(GetProgramFiles(), LOCAL_GLOBAL_INSTALLATION_LOCATION);
        }

        public static string GetUpdatesVersionFile()
        {
            return UPDATES_VERSION_FILENAME;
        }

        public static string GetUpdatesSetupFile()
        {
            return UPDATES_SETUP_FILENAME;
        }

        public static int GetTimeBetweenChecks()
        {
            return TIME_BETWEEN_CHECKS;
        }

        #endregion

        public static string GetStationsSummaryLocation(Facilities facility)
        {
            return Path.Combine(GetStationsSummaryGlobalLocation(facility), STATION_SUMMARY_LOCATION);
        }

        public static string GetUpdatesLocation(Facilities facility)
        {
            return Path.Combine(GetUpdatesGlobalLocation(facility), UPDATES_LOCATION);
        }
        
        public static string GetLocalInstallationFolder()
        {
            return Path.Combine(GetLocalGlobalInstallationLocation(), LOCAL_INSTALLATION_FOLDER);
        }

        private static string GetProgramFiles()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }
    }
}