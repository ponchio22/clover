using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Valutech.Electrox.Data
{
    class LocalSettingsManager
    {
        private static string FILENAME = "LocalSettings.xml";

        private const string SETTINGS_TAG = "Settings";

        private const string PLANT_TAG = "Plant";

        private const string AREA_TAG = "Area";

        private static LocalSettingsManager instance;

        private string plant = String.Empty;

        private string area = String.Empty;

        private LocalSettingsManager() { }

        private bool loaded = false;

        private XElement settingsElement = null;

        public static LocalSettingsManager GetInstance()
        {
            if (instance == null) instance = new LocalSettingsManager();
            return instance;
        }

        public bool LoadData()
        {
            try
            {
                if (!File.Exists(FullPath))
                {
                    settingsElement = new XElement(SETTINGS_TAG,
                        new XElement(PLANT_TAG, "[All]"),
                        new XElement(AREA_TAG, "[All]")
                        );
                    settingsElement.Save(FullPath);

                }
                if (File.Exists(FullPath))
                {
                    XDocument doc = XDocument.Load(FullPath);
                    settingsElement = doc.Elements(SETTINGS_TAG).Single();
                    this.plant = settingsElement.Elements(PLANT_TAG).Single().Value;
                    this.area = settingsElement.Elements(AREA_TAG).Single().Value;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
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

        public string Plant
        {
            get
            {
                if (!loaded) LoadData();
                return this.plant;
            }
            set
            {
                this.plant = value;
                settingsElement.Elements(PLANT_TAG).Single().Value = this.plant;
                settingsElement.Save(FullPath);                
            }
        }

        public string Area
        {
            get
            {
                if (!loaded) LoadData();
                return this.area;
            }
            set
            {
                this.area = value;
                settingsElement.Elements(AREA_TAG).Single().Value = this.area;
                settingsElement.Save(FullPath);                
            }
        }
    }
}
