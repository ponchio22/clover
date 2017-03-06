using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class FileSyncManagerConfiguration
    {
        public static string PROCESS_NAME = "FileSyncManager";

        public static string APPLICATION_FRIENDLY_NAME = "FileSync Manager";

        private static string ONLINE_DIRECTORY = "FileSyncManager";

        private static string LOCAL_INSTALLATION_FOLDER = "FileSyncManager";

        private static string FILE_NAME = "FileSyncManager.exe";
        
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
    }
}