using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Valutech.IO;

namespace Valutech.FileSync
{
    public class NetworkIniFile : INIFile
    {

        private const string ORIGIN = "Origin";

        private const string DESTINY = "Destiny";

        public NetworkIniFile(string configFile)
            : base(configFile)
        {
            
        }

        public SyncItem getSyncItem(string name)
        {
            SyncItem item = new SyncItem(name, Read(name, ORIGIN), Read(name, DESTINY));
            if (item.origin == String.Empty || item.destiny == String.Empty)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public List<SyncItem> getSyncItems(ArrayList names)
        {
            List<SyncItem> list = new List<SyncItem>();
            SyncItem item;
            foreach (String name in names)
            {
                item = getSyncItem(name);
                if (item != null)
                {
                    list.Add(new SyncItem(item.name, item.origin, item.destiny));
                }
            }
            return list;
        }

        public List<SyncItem> getSyncItems()
        {          
            ArrayList profiles = new ArrayList();
            profiles = this.GetSectionNames();
            return getSyncItems(profiles);
        }
    }
}