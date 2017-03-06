using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class EarpieceTestConfiguration
    {
        public static string PROCESS_NAME = "EarpieceTest";

        public static string APPLICATION_FRIENDLY_NAME = "EarpieceTest";

        private static string ONLINE_DIRECTORY = "EarpieceTest";

        private static string LOCAL_INSTALLATION_FOLDER = "EarpieceTest";

        private static string IPA_ONLINE_DIRECTORY = "EarpieceTestIPA";

        private static string FILE_NAME = "EarpieceTest.exe";

        private static string INSTALLATION_PATH = Path.Combine(@"C:\Program Files\",LOCAL_INSTALLATION_FOLDER);

        private static string LOGS_PATH = @"\\datafeed-new\Valutech\ET\SOURCE\";

        public static string GetOnlineSourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceGlobalLocation(facility), ONLINE_DIRECTORY);
        }

        public static string GetInstallationFolder()
        {
            return INSTALLATION_PATH;
        }

        public static string GetApplicationPath()
        {
            return Path.Combine(GetInstallationFolder(), FILE_NAME);
        }

        public static string GetLogsFolder()
        {
            return Path.Combine(LOGS_PATH);
        }

        /// <summary>
        /// Ipa Folder
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        public static string GetOnlineIPASourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceGlobalLocation(facility), IPA_ONLINE_DIRECTORY);
        }
    }
}