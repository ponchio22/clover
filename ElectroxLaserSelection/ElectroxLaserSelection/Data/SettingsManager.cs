using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Valutech.Electrox
{
    class SettingsManager
    {
        private static string FILENAME = "Settings.xml";

        private static SettingsManager instance;

        private SettingsManager() { }

        private const string SETTINGS_TAG = "Settings";

        private const string MARKER_INI_FILE_SOURCE_TAG = "MarkerIniFileSource";

        private const string LASER_LIST_FILE_SOURCE_TAG = "LaserListFileSource";

        private const string FONT_AUTODOWNLOAD_FILE_SOURCE_TAG = "FontAutoDownloadFileSource";

        private string markerIniFileSource;

        private string laserListFileSource;

        private string fontAutoDownloadFileSource;

        private string plant;

        private string area;

        private bool loaded = false;

        public static SettingsManager GetInstance()
        {
            if (instance == null) instance = new SettingsManager();
            return instance;
        }

        public bool LoadData()
        {
            if (File.Exists(FullPath))
            {
                XDocument doc = XDocument.Load(FullPath);
                var data = (from e in doc.Descendants(SETTINGS_TAG)
                            select new
                            {
                                markerIniFileSource = (string) e.Element(MARKER_INI_FILE_SOURCE_TAG).Value,
                                laserListFileSource = (string) e.Element(LASER_LIST_FILE_SOURCE_TAG).Value,
                                fontAutoDownloadFileSource = (string) e.Element(FONT_AUTODOWNLOAD_FILE_SOURCE_TAG).Value
                            }).Single();
                this.markerIniFileSource = data.markerIniFileSource;
                this.laserListFileSource = data.laserListFileSource;
                this.fontAutoDownloadFileSource = data.fontAutoDownloadFileSource;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the full path of the file, it should be the same application path
        /// </summary>
        public string FullPath
        {
            get
            {
                return Path.Combine(Application.StartupPath, FILENAME);
            }
        }

        public string MarkerIniFileSource
        {
            get
            {
                if (!loaded) LoadData();
                return this.markerIniFileSource;
            }
        }

        public string LaserListFileSource
        {
            get
            {
                if (!loaded) LoadData();
                return this.laserListFileSource;
            }
        }

        public string FontAutoDownloadFileSource
        {
            get
            {
                if (!loaded) LoadData();
                return this.fontAutoDownloadFileSource;
            }
        }

        
    }
}
