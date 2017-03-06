using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Collections;

namespace Valutech.RF
{
    public class Agilent
    {
        /// <summary>
        /// Gets the serial number of the agilent on a RF Station
        /// </summary>
        /// <returns>String with the serial number</returns>
        public static string GetSN()
        {
            //Check the registry for installed agilent versions
            RegistryKey wtmKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Agilent\\Wireless\\Wireless Test Manager Selectable Applications");
            DateTime newestAccessTime = new DateTime(1900,1,1);
            DateTime lastAccessTime;
            string serialNumber = String.Empty;
            if (wtmKey != null)
            {
                string[] versions = wtmKey.GetSubKeyNames();
                ArrayList foundSN = new ArrayList();
                if (versions.Length > 0)
                {
                    RegistryKey versionKey;
                    string execPath = String.Empty;
                    foreach (string version in versions)
                    {
                        versionKey = wtmKey.OpenSubKey(version + "\\CurrentVersion");
                        if (versionKey != null)
                        {
                            execPath = versionKey.GetValue("Path").ToString();                            
                            Match match = new Regex(version).Match(execPath);
                            if (match != null)
                            {
                                //Get the paths
                                string fileName = execPath.Substring(0, match.Index) + version + "\\TestData\\ExecSetting.xml";
                                if (File.Exists(fileName))
                                {
                                    //Load the exec settings for all the version
                                    try
                                    {
                                        XmlDocument xml = new XmlDocument();
                                        xml.Load(@fileName);
                                        //Get the serial from the files
                                        XmlNodeList list = xml.DocumentElement.GetElementsByTagName("test_set_serial_number");
                                        if (list.Count > 0)
                                        {
                                            //Discard files with empty serial no
                                            if (list[0].InnerText != String.Empty)
                                            {
                                                if (File.Exists(execPath))
                                                {
                                                    lastAccessTime = File.GetLastAccessTime(execPath);
                                                    if (DateTime.Compare(lastAccessTime, newestAccessTime) > 0)
                                                    {
                                                        serialNumber = list[0].InnerText;
                                                        newestAccessTime = lastAccessTime;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            XmlNodeList exec1 = xml.DocumentElement.GetElementsByTagName("Exec1");
                                            if (exec1.Count > 0)
                                            {
                                                XmlNode testserialnode = xml.CreateNode(XmlNodeType.Element, "test_set_serial_number", null);                                                
                                                testserialnode.InnerText = "FirstTime";
                                                exec1[0].AppendChild(testserialnode);
                                                xml.Save(@fileName);
                                            }
                                        }
                                    }
                                    catch {}
                                }
                            }
                        }
                    }
                    return serialNumber;
                }
            }            
            return String.Empty;
        }

    }
}
