using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class EarpieceTestLogUploadServiceConfiguration
    {
        public static string PROCESS_NAME = "ET_service_local";

        public static string APPLICATION_FRIENDLY_NAME = "Valutech_ET_service";

        private static string ONLINE_DIRECTORY = "Earpiece Service";

        private static string LOCAL_INSTALLATION_FOLDER = "service_install";

        private static string FILE_NAME = "ET_service_local.exe";

        private static string INSTALLATION_FOLDER = @"C:\Program Files\EarpieceTest\";

        private static string INSTALLER_FILE_NAME = "Uninstall.bat";


        public static string GetOnlineSourceLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatedSourceITGlobalLocation(facility), ONLINE_DIRECTORY);
        }

        public static string GetInstallationFolder()
        {
            return Path.Combine(INSTALLATION_FOLDER, LOCAL_INSTALLATION_FOLDER);
        }

        public static string GetApplicationPath()
        {
            return Path.Combine(GetInstallationFolder(), FILE_NAME);
        }

        public static string GetInstallerPath()
        {
            return Path.Combine(GetInstallationFolder(), INSTALLER_FILE_NAME);
        }
    }
}