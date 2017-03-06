using System;
using System.Collections;
using System.IO;

namespace Valutech.Station
{
    /// <summary>
    /// Manages all the stations locations and upload new versions and update hash at the same time
    /// </summary>
    public class StationsManager
    {
        /// <summary>
        /// Variable that holds the instance of the program
        /// </summary>
        private static StationsManager instance;

        /// <summary>
        /// Gets the unique instance of the object
        /// </summary>
        /// <returns></returns>
        public static StationsManager GetInstance()
        {
            if (instance == null) instance = new StationsManager();
            return instance;
        }

        /// <summary>
        /// Gets the station types
        /// </summary>
        /// <returns>ArrayList of StationsManagerUpdatesStation objects</returns>
        public ArrayList GetStationTypes(StationsManagerStationConfiguration.Facilities facility)
        {
            ArrayList stationTypes = new ArrayList();
            try
            {                
                string updatesLocation = StationsManagerStationConfiguration.GetUpdatesGlobalLocation(facility);
                string[] directories = Directory.GetDirectories(updatesLocation);
                foreach (string directory in directories)
                {
                    StationsManagerUpdatesStation station = new StationsManagerUpdatesStation(directory, facility);
                    if (station.isValid)
                    {
                        stationTypes.Add(station);
                    }
                }
            }
            catch
            {

            }
            return stationTypes;
        }
    }
}