using System;
using System.IO;

namespace Valutech.Station
{
    public class EngineeringStationConfiguration
    {
        private static string SUMMARY_LOCATION_PATH = @"ET\Stations\";

        private static string UPDATES_LOCATION_PATH = @"EngineeringStationSetup\";

        /// <summary>
        /// Gets the place to upload the station information
        /// </summary>
        /// <returns></returns>
        public static string GetStationsSummaryLocation(StationsManagerStationConfiguration.Facilities facility)
        {
            return Path.Combine(StationsManagerStationConfiguration.GetStationsSummaryGlobalLocation(facility), SUMMARY_LOCATION_PATH);
        }

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