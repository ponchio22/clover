using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Valutech.Agilent
{
    public delegate void WTMVersionOpenEventHandler(WTMVersion version);

    public delegate void WTMVersionClosedEventHandler(WTMVersion version);

    public class WTMVersion
    {
        private string name;

        private ArrayList oems = new ArrayList();

        /// <summary>
        /// Path not found
        /// </summary>
        public const string PATH_NOT_FOUND = "not_found";

        /// <summary>
        /// Version process not found
        /// </summary>
        public const string PROCESS_NOT_FOUND = "process_not_found";

        public event WTMVersionOpenEventHandler Opened;

        public event WTMVersionClosedEventHandler Closed;

        public WTMVersion(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Opens the current version of the WTM
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                string ProcessPath = this.ProcessPath;
                if (ProcessPath != PROCESS_NOT_FOUND)
                {
                    if (!IsRunning()) Process.Start(ProcessPath);
                    if (Opened != null) Opened(this);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tells you if this version of the wtm is currently running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            try
            {
                bool result = (Process.GetProcessesByName(this.ProcessName).Length > 0);
                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the defined version of the wtm
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool Close()
        {
            if (this.IsRunning())
            {
                Process[] processList = Process.GetProcessesByName(this.ProcessName);
                foreach (Process p in processList) p.Kill();
            }
            int seconds = 0;
            while (this.IsRunning() && seconds < 5)
            {
                Thread.Sleep(1000);
                seconds++;
            }
            if (seconds >= 5)
            {
                return false;
            }
            else
            {
                if (Closed != null) Closed(this);
                return true;
            }
        }
        

        #region General Properties

        /// <summary>
        /// Returns the Name of the version
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Override for the ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name;
        }

        #endregion

        #region Process Properties

        /// <summary>
        /// Gets the process name of the version
        /// </summary>
        public string ProcessName
        {
            get
            {
                Regex regexp = new Regex("[^\\\\]{1,}\\.(exe)$");
                MatchCollection matches = regexp.Matches(ProcessPath);
                if (matches.Count > 0)
                {
                    return matches[0].Value.Replace(".exe", "");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Gets the process path of the version
        /// </summary>
        public string ProcessPath {
            get
            {
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(WirelessTestManager.WTM_VERSIONS_REGISTRY_PATH + "\\" + Name + "\\CurrentVersion", false);
                if (regkey != null)
                {
                    return regkey.GetValue(WirelessTestManager.WTM_REGISTRY_PATH_PATH_NAME, PROCESS_NOT_FOUND).ToString();
                }
                return string.Empty;
            }
        }

        #endregion

        #region OEMs MInformation Retrieval

        /// <summary>
        /// Gets the already loaded oems for the specified version
        /// </summary>
        /// <returns>ArrayList with the OEM Objects found</returns>
        public ArrayList GetOEMs()
        {
            if (oems.Count == 0) LoadOEMs();
            return oems;
        }

        /// <summary>
        /// Get the oem object from the name
        /// </summary>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public OEM GetOEM(string oemName)
        {
            ArrayList oems = LoadOEMs();
            foreach (OEM oem in oems)
            {
                if (oem.Name == oemName) return oem;
            }
            return null;
        }

        /// <summary>
        /// Load the oems for the specified version
        /// </summary>
        /// <returns>ArrayList with the OEM Objects found</returns>
        public ArrayList LoadOEMs()
        {
            try
            {
                oems = new ArrayList();
                string path = WirelessTestManager.GetInstance().GetPath();
                if (path != PATH_NOT_FOUND)
                {
                    string[] dirs = Directory.GetDirectories(path);
                    Regex regexp = new Regex("[^\\\\]{1,}\\\\" + this.Name);
                    foreach (string dir in dirs)
                    {
                        if (regexp.IsMatch(dir))
                        {
                            OEM oem = new OEM(dir, this);
                            oems.Add(oem);
                        }
                    }
                }
                return oems;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the oem currently in use by the version set from the already loaded oems
        /// </summary>
        /// <returns></returns>
        public OEM GetOEMInUse()
        {
            oems = this.GetOEMs();
            foreach (OEM oem in oems)
            {
                if (oem.InUse) return oem;
            }
            return null;
        }

        #endregion
    }
}
