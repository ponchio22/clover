using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Valutech.IO;
using System.IO;
using Valutech.Configurations;

namespace Valutech.FileSync
{
    public class DBInfoIniFile : INIFile
    {

        /// <summary>
        /// Config file
        /// </summary>
        public static string FILE_NAME = ValutechFileSyncConfiguration.DB_INFO;

        private const string SYNC_SECTION = "Sync Items List";

        private const string SYNC_CONFIG_SECTION = "Sync Configuration";

        private const string SYNC_DESCRIPTION_FILE = "Description List File";

        private const string SYNCITEMPREFIX = "SyncItem";

        private const string SYNC_ROOT = "Root";
/*
        private const string DEFAULT_ROOT_VALUE = @"\\file-server\files\49_ENG_TOOLS\SyncFiles";
        */

        public DBInfoIniFile()
            : base(Path.Combine(ValutechFileSyncConfiguration.GetInstallationFolder(),FILE_NAME))
        {
            /*
            if (Read(SYNC_SECTION, SYNCITEMPREFIX + "0") == string.Empty)
            {
                Write(SYNC_SECTION, SYNCITEMPREFIX + "0", "dummy");               
            }
            if (Read(SYNC_CONFIG_SECTION, SYNC_DESCRIPTION_FILE) == string.Empty)
            {
                Write(SYNC_CONFIG_SECTION, SYNC_DESCRIPTION_FILE, "dummy");
            }.
            */
        }


        public string getDescriptionListFile()
        {
            return descriptionListFile;
        }

        public string descriptionListFile
        {
            set { Write(SYNC_CONFIG_SECTION, SYNC_DESCRIPTION_FILE,value); }
            get { return Read(SYNC_CONFIG_SECTION, SYNC_DESCRIPTION_FILE); }
        }

        /// <summary>
        /// Gets the value of the root of all the sync profiles
        /// </summary>
        public string SyncRootDirectoryPath
        {
            set { Write(SYNC_CONFIG_SECTION, SYNC_ROOT, value); }
            get 
            { 
                string returnValue = Read(SYNC_CONFIG_SECTION, SYNC_ROOT);   /*  
                if(returnValue==String.Empty) {
                    Write(SYNC_CONFIG_SECTION,SYNC_ROOT,DEFAULT_ROOT_VALUE);
                    returnValue = DEFAULT_ROOT_VALUE;
                }*/
                return returnValue;
            }
        }

        /// <summary>
        /// Add a profile to the list in the ini file
        /// </summary>
        /// <param name="profileName">Name of the profile</param>
        public void addProfile(string profileName)
        {
            ArrayList names = getSyncItemsName();
            bool profileNameFound = false;
            long count = 0;
            foreach (String name in names)
            {
                if (name == profileName) profileNameFound = true;
                count++;
            }
            if (!profileNameFound)
            {
                Write(SYNC_SECTION, SYNCITEMPREFIX + count, profileName);
            }
        }

        /// <summary>
        /// Remove profile from the list in the ini file
        /// </summary>
        /// <param name="profileName"></param>
        public void removeProfile(string profileName)
        {
            int count = 0;
            ArrayList list = getSyncItemsName();
            ArrayList newList = new ArrayList();
            foreach (string item in list)
            {
                if (item != profileName)
                {
                    newList.Add(item);
                }
                Write(SYNC_SECTION, SYNCITEMPREFIX + count, null);
                count++;
            }
            count = 0;
            foreach (string item in newList)
            {
                Write(SYNC_SECTION, SYNCITEMPREFIX + count, item);
                count++;
            }
        }

        /// <summary>
        /// Get list of profiles to sync
        /// </summary>
        /// <returns>List with the profiles to be synced</returns>
        public ArrayList getSyncItemsName()
        {
            long emptyCount = 0;
            long count = 0;
            ArrayList list = new ArrayList();
            String name;
            do
            {
                name = Read(SYNC_SECTION, SYNCITEMPREFIX + count);
                if (name != string.Empty)
                {
                    list.Add(name);
                    emptyCount = 0;
                }
                else
                {
                    emptyCount++;
                }
                count++;
            } while (emptyCount < 1);
            return list;
        }
    }
}