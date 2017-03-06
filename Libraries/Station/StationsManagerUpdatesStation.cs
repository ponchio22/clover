using System;
using System.IO;

namespace Valutech.Station
{
    public class StationsManagerUpdatesStation
    {
        private string updatesPath;

        private StationsManagerStationConfiguration.Facilities facility;

        private string name;

        public StationsManagerUpdatesStation(string updatesPath,StationsManagerStationConfiguration.Facilities facility)
        {
            this.UpdatesPath = updatesPath.EndsWith(@"\") ? updatesPath : updatesPath + @"\"; ;
            this.facility = facility;
        }

        public bool isValid
        {
            get
            {
                return File.Exists(Path.Combine(this.updatesPath, StationsManagerStationConfiguration.GetUpdatesVersionFile()));
            }
        }

        public string UpdatesPath
        {
            set
            {
                this.updatesPath = value;
                this.name = this.updatesPath.Replace(StationsManagerStationConfiguration.GetUpdatesGlobalLocation(this.facility), string.Empty).TrimEnd('\\');
            }
            get
            {
                return this.updatesPath;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}