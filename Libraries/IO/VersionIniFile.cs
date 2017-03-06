using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Valutech.Station;

namespace Valutech.Sync
{
    public class VersionIniFile
    {
        public static string FILE_NAME = StationsManagerStationConfiguration.GetUpdatesVersionFile();

        private string sectionTag = "Configuration";

        private string versionTag = "Version";

        private string fullPath;

        private XDocument xml;

        public VersionIniFile(string path)            
        {
            fullPath = path + @"\" + FILE_NAME;
            if (!File.Exists(fullPath))
            {
                xml = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement(sectionTag,
                            new XElement(versionTag, "0.0.0.0")
                            )
                        );
                xml.Save(fullPath);
                xml = null;                
            }            
        }

        public string Path
        {
            get
            {
                return fullPath;
            }
        }    

        /// <summary>
        /// Gets or Sets the version of the file
        /// </summary>
        public string Version
        {
            get
            {
                xml = XDocument.Load(fullPath);
                string value = xml.Descendants(versionTag).First().Value.ToString();
                xml = null;
                return value;
            }
            set
            {
                xml = XDocument.Load(fullPath);
                xml.Descendants(versionTag).First().Value = value;
                xml.Save(fullPath);
                xml = null;
            }
        }
    }
}
