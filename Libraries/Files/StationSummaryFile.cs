using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Valutech.RF;

namespace Valutech.Sync
{
    public class StationSummaryFile
    {
        private string station;

        private string informationTag = "Information";

        private string ipTag = "Ip";

        private string computerNameTag = "ComputerName";

        private string lastUpdate = "LastUpdate";

        private string versionTag = "Version";

        private string typeTag = "Type";

        private string serialNumberTag = "SN";

        private string path;

        public StationSummaryFile()
        {
            this.station = System.Environment.MachineName;
        }

        public StationSummaryFile(string path)
        {
            this.path = path;            
        }

        public void Save(string path)
        {
            this.path = path;
            if (Directory.Exists(path))
            {
                string filename = path + @"\" + this.station + ".xml";
                XDocument doc;
                try
                {                      
                    doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(informationTag,
                        new XElement(ipTag, GetIP()),
                        new XElement(computerNameTag,this.station),
                        new XElement(lastUpdate,DateTime.Now),
                        new XElement(versionTag,GetVersion()),
                        new XElement(typeTag,GetType()),
                        new XElement(serialNumberTag,Agilent.GetSN())
                        )
                    );
                    doc.Save(filename);                        
                }
                catch
                {
                    Console.Write("Error");
                }
            }
        }

        public string ComputerName
        {
            get
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
                    return doc.Descendants(computerNameTag).First().Value.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string IP
        {
            get
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
                    return doc.Descendants(ipTag).First().Value.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string Type
        {
            get
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
                    return doc.Descendants(typeTag).First().Value.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string LastUpdate
        {
            get
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
                    return doc.Descendants(lastUpdate).First().Value.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string SerialNumber
        {
            get
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
                    return doc.Descendants(serialNumberTag).First().Value.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        private string GetIP()
        {
            //Get ip
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public string GetVersion()
        {
            //Get version
            string version = String.Empty;
            RegistryKey subkey = Registry.LocalMachine.OpenSubKey(UpdaterService.UPDATES_SUBKEY);
            if (subkey != null)
            {
                object path = subkey.GetValue(UpdaterService.LOCAL_LOCATION_KEY_NAME);
                if (path != null)
                {
                    VersionIniFile versionFile = new VersionIniFile((string) path);
                    version = versionFile.Version;
                }
            }
            return version;
        }

        private new string GetType()
        {
            //Get version
            string type = String.Empty;
            RegistryKey subkey = Registry.LocalMachine.OpenSubKey(UpdaterService.UPDATES_SUBKEY);
            if (subkey != null)
            {
                string updatesPath = (string)subkey.GetValue(UpdaterService.UPDATES_LOCATION_KEY_NAME);
                Regex reg = new Regex(@"[^\\]{1,}$");
                Match match = reg.Match(updatesPath.Substring(0, updatesPath.Length - 1));
                type = match.ToString();
            }
            return type;
        }
    }
}
