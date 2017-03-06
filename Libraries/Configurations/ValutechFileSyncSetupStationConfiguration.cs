using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class ValutechFileSyncSetupStationConfiguration
    {
        public static string PROCESS_NAME = "ValutechFileSyncSetupStation";

        public static string APPLICATION_FRIENDLY_NAME = "ValutechFileSyncSetupStation";

        private static string ONLINE_DIRECTORY = "ValutechFileSync";

        private static string LOCAL_INSTALLATION_FOLDER = "ValutechFileSync";

        private static string FILE_NAME = "ValutechFileSyncSetupStation.exe";

        public static string GetOnlineSourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceGlobalLocation(facility), ONLINE_DIRECTORY);
        }

        public static string GetInstallationFolder()
        {
            return Path.Combine(StationsManagerStationConfiguration.GetLocalGlobalInstallationLocation(), LOCAL_INSTALLATION_FOLDER);
        }

        public static string GetOnlineApplicationPath(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), FILE_NAME);
        }

        public static string GetApplicationPath()
        {
            return Path.Combine(GetInstallationFolder(), FILE_NAME);
        }
    }
}