using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalSettingsManagerLib;

namespace ElectroxFixtureSelectionLib
{
    public class ElectroxFixtureSelectionLocalSettings:LocalSettingsManager
    {

        private const string LASER_TAG = "SelectedLaser";

        /// <summary>
        /// Constructor, add the necessary fields in the xml file
        /// </summary>
        public ElectroxFixtureSelectionLocalSettings()
            : base()
        {
            this.AddField(new LocalSettingsManagerField(LASER_TAG, String.Empty));
        }

        /// <summary>
        /// Getter setter for the selected laser
        /// </summary>
        public string SelectedLaser
        {
            set
            {
                this.SetValue(LASER_TAG, value);
            }
            get
            {
                return this.GetValue(LASER_TAG);
            }
        }
    }
}
