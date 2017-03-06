using System;
using System.IO;
using Valutech.Station;

namespace Valutech.Configurations
{
    public class SamsungSWConfiguration
    {

        private static string UPDATES_LOCATION_PATH = @"SamsungSWStationsSetup\";

        /// <summary>
        /// Place to search for updates online
        /// </summary>
        /// <returns></returns>
        public static string GetUpdatesLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetUpdatesGlobalLocation(facility), UPDATES_LOCATION_PATH);
        }

    }
}