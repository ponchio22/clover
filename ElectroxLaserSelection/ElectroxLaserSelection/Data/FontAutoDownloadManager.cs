using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace Valutech.Electrox
{
    class FontAutoDownloadManager
    {
        private SettingsManager settings = SettingsManager.GetInstance();

        private const string FONT_AUTO_DOWNLOAD_TAG = "FontAutoDownload";

        private const string FONT_TAG = "Font";

        private static FontAutoDownloadManager instance;

        private List<string> fonts;

        private bool loaded = false;

        private FontAutoDownloadManager() { }

        public static FontAutoDownloadManager GetInstance()
        {
            if (instance == null) instance = new FontAutoDownloadManager();
            return instance;
        }

        public bool LoadData()
        {
            if (File.Exists(FullPath))
            {
                XDocument doc = XDocument.Load(FullPath);
                fonts = (from e in doc.Descendants(FONT_TAG)
                         select e.Value).ToList();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the full path of the file, it should be the same application path
        /// </summary>
        private string FullPath
        {
            get
            {
                return settings.FontAutoDownloadFileSource;
            }
        }

        public List<string> Fonts
        {
            get
            {
                if (!loaded) LoadData();
                return this.fonts;
            }
        }
    }
}
