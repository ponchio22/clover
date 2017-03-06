using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class EarpieceQCValidationConfiguration
    {
        public static string PROCESS_NAME = "EarpieceQCValidation";

        public static string APPLICATION_FRIENDLY_NAME = "Earpiece QC Validation";

        private static string ONLINE_DIRECTORY = "EarpieceQCValidation";

        private static string LOCAL_INSTALLATION_FOLDER = "EarpieceQCValidation";

        private static string FILE_NAME = "EarpieceQCValidation.exe";

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