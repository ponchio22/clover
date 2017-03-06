using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class UpdatePlansDirectoryConfiguration
    {
        public static string PROCESS_NAME = "UpdatePlansDirectory";

        public static string APPLICATION_FRIENDLY_NAME = "UpdatePlansDirectory";

        private static string ONLINE_DIRECTORY = @"UpdatePlansDirectory";

        private static string LOCAL_INSTALLATION_FOLDER = "UpdatePlansDirectory";

        private static string FILE_NAME = "UpdatePlansDirectory.exe";

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