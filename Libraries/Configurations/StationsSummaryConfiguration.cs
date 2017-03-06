using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class StationsSummaryConfiguration
    {
        public static string PROCESS_NAME = "StationsSummary";

        public static string APPLICATION_FRIENDLY_NAME = "StationsSummary";

        private static string ONLINE_DIRECTORY = @"StationsSummary";

        private static string LOCAL_INSTALLATION_FOLDER = "StationsSummary";

        private static string FILE_NAME = "StationsSummary.exe";

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