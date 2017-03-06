using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class RFControlConfiguration
    {
        public static string PROCESS_NAME = "RF_Control_Plus";

        public static string APPLICATION_FRIENDLY_NAME = "RF Control Plus";

        private static string ONLINE_DIRECTORY = "RFControlPlus";

        private static string LOCAL_INSTALLATION_FOLDER = "";

        private static string FILE_NAME = "RF_Control_Plus.application";

        public static string GetOnlineSourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceITGlobalLocation(facility), ONLINE_DIRECTORY);
        }

        public static string GetInstallationFolder()
        {
            return Path.Combine(StationsManagerStationConfiguration.GetLocalGlobalInstallationLocation(), LOCAL_INSTALLATION_FOLDER);
        }

        public static string GetApplicationPath()
        {
            return Path.Combine(GetInstallationFolder(), FILE_NAME);
        }

        public static string GetOnlineApplicationPath(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), FILE_NAME);
        }

    }
}