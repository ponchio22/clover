using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace LocalSettingsManagerLib
{
    
    public class LocalSettingsManager
    {

        private string filename = string.Empty;

        public const string SETTINGS_TAG = "Settings";

        private List<LocalSettingsManagerField> fields = new List<LocalSettingsManagerField>();

        private XElement settingsElement = null;

        private bool loaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename"></param>
        public LocalSettingsManager(string filename)
        {
            this.filename = filename;            
        }

        public LocalSettingsManager()
        {
            this.filename = "LocalSettings.xml";
        }

        /// <summary>
        /// Loads the local settings data
        /// </summary>
        /// <returns></returns>
        public bool LoadData(bool forceLoad)
        {
            try
            {
                if (!loaded || forceLoad)
                {
                    if (!File.Exists(this.FullPath))
                    {
                        settingsElement = new XElement(SETTINGS_TAG);
                        foreach (LocalSettingsManagerField field in fields) settingsElement.Add(new XElement(field.fieldName, field.value));
                        settingsElement.Save(FullPath);
                    }
                    else
                    {
                        XDocument doc = XDocument.Load(FullPath);
                        settingsElement = doc.Elements(SETTINGS_TAG).Single();
                        foreach (LocalSettingsManagerField field in fields) field.value = settingsElement.Elements(field.fieldName).Single().Value;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Loads the data even if its already been loaded
        /// </summary>
        /// <returns></returns>
        public bool LoadData()
        {
            return LoadData(true);
        }

        /// <summary>
        /// Adds a field to the list of settings
        /// </summary>
        /// <param name="field"></param>
        protected void AddField(LocalSettingsManagerField field)
        {
            this.fields.Add(field);
        }
        
        /// <summary>
        /// Gets the field that matches the name supplied
        /// </summary>
        /// <param name="name">Name of the field to get</param>
        /// <returns></returns>
        protected LocalSettingsManagerField GetField(string name)
        {
            foreach (LocalSettingsManagerField field in this.fields)
            {
                if (field.fieldName == name) return field;
            }
            return null;
        }

        /// <summary>
        /// Set the value of the field matching the name
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="value"></param>
        protected void SetValue(string fieldname, string value)
        {
            this.LoadData();
            this.GetField(fieldname).value = value;
            settingsElement.Elements(fieldname).Single().Value = value;
            settingsElement.Save(FullPath);
        }

        /// <summary>
        /// Get the value of the field matching the name
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        protected string GetValue(string fieldname)
        {
            try
            {
                LoadData(false);
                return this.GetField(fieldname).value;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the full path of the file, it should be the same application path
        /// </summary>
        public string FullPath
        {
            get
            {
                return Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), filename);
            }
        }
    }
}
