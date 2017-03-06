using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class WTMToolConfiguration
    {
        public static string PROCESS_NAME = "WTMTool";

        public static string APPLICATION_FRIENDLY_NAME = "WTMTool";

        private static string ONLINE_DIRECTORY = @"WTMTool\WTMTool";

        private static string LOCAL_INSTALLATION_FOLDER = "WTMTool";

        private static string FILE_NAME = "WTMTool.exe";

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