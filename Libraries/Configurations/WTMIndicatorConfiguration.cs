using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class WTMIndicatorConfiguration
    {
        public static string PROCESS_NAME = "WTMIndicator";

        public static string EXTENSION = ".exe";

        public static string APPLICATION_FRIENDLY_NAME = "WTMIndicator";

        private static string ONLINE_DIRECTORY = "RFControl";

        private static string LOCAL_INSTALLATION_FOLDER = "RFControl";

        private static string FILE_NAME = PROCESS_NAME + EXTENSION;

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