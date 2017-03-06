using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class ValutechFileSyncConfiguration
    {
        public static string PROCESS_NAME = "ValutechFileSync";

        public static string APPLICATION_FRIENDLY_NAME = "ValutechFileSync";

        private static string ONLINE_DIRECTORY = "ValutechFileSync";

        private static string LOCAL_INSTALLATION_FOLDER = "ValutechFileSync";

        private static string FILE_NAME = "ValutechFileSync.exe";

        private static string INSTALLER_FILE_NAME = "Installer.bat";

        private static string INSTALL_UTIL_FILE = "InstallUtil.exe";

        private static string INI_FILE = "ValutechFileSync.ini";

        public static string DB_INFO = "DBInfo.ini";

        public static string SYNC_FILES_FOLDER = "SyncFiles";

        public static string SYNC_DATA_FILE = "SyncData.ini";

        public static string SUBKEY_PATH = @"SOFTWARE\Valutech\Updater";

        public static string UPDATED_SOURCE_KEY = "UpdatedSource";

        public static string SYNC_DATA_FILE_KEY = "SyncDataFile";

        public static string ET_CONFIG_FILES_DIRECTORY = "ETStation";

        public static string SAMSUNG_SW_CONFIG_FILES_DIRECTORY = "SamsungSWStation";

        public static string SAMSUNG_CLEAR_CONFIG_FILES_DIRECTORY = "SamsungClearStation";

        public static string GetOnlineSyncDataFileLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetFileSyncUpdatedSyncDataGlobalLocation(facility), SYNC_DATA_FILE);
        }

        public static string GetOnlineSyncFilesLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetFileSyncUpdaterSourceGlobalLocation(facility), SYNC_FILES_FOLDER);
        }

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

        public static string GetOnlineInstallerPath(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), INSTALLER_FILE_NAME);
        }

        public static string GetOnlineInstallUtilPath(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), INSTALL_UTIL_FILE);
        }

        public static string GetInstallerPath()
        {
            return Path.Combine(GetInstallationFolder(), INSTALLER_FILE_NAME);
        }

        public static string GetInstallUtilPath()
        {
            return Path.Combine(GetInstallationFolder(), INSTALL_UTIL_FILE);
        }

        #region Ini File

        public static string GetOnlineIniFile(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), ET_CONFIG_FILES_DIRECTORY, INI_FILE);
        }

        public static string GetSamsungSWOnlineIniFile(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), SAMSUNG_CLEAR_CONFIG_FILES_DIRECTORY, INI_FILE);
        }

        public static string GetSamsungClearOnlineIniFile(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), SAMSUNG_CLEAR_CONFIG_FILES_DIRECTORY, INI_FILE);
        }

        public static string GetIniFile()
        {
            return Path.Combine(GetInstallationFolder(), INI_FILE);
        }

        #endregion

        #region DB Info

        public static string GetOnlineDBInfo(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility),ET_CONFIG_FILES_DIRECTORY, DB_INFO);
        }

        public static string GetSamsungSWOnlineDBInfo(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), SAMSUNG_SW_CONFIG_FILES_DIRECTORY, DB_INFO);
        }

        public static string GetSamsungClearOnlineDBInfo(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(GetOnlineSourceLocation(facility), SAMSUNG_CLEAR_CONFIG_FILES_DIRECTORY, DB_INFO);
        }

        public static string GetDBInfo()
        {
            return Path.Combine(GetInstallationFolder(), DB_INFO);
        }

        #endregion
    }
}