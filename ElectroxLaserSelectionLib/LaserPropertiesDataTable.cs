using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Valutech.Electrox
{
    public class LaserPropertiesDataTable : DataTable
    {
        #region Constants

        public const string PROPERTY_COLUMN = "Properties";

        public const string VALUES_COLUMN = "Values";

        public const string NAME_ROW = "Name";

        public const string PLANT_ROW = "Plant";

        public const string AREA_ROW = "Area";

        public const string IP_ROW = "Ip";

        public const string DSP_ROW = "Dsp";

        #endregion

        private DataRow nameRow;
        private DataRow areaRow;
        private DataRow plantRow;
        private DataRow ipRow;
        private DataRow dspRow;

        public LaserPropertiesDataTable()
        {
            this.Columns.Add(PROPERTY_COLUMN);
            this.Columns.Add(VALUES_COLUMN);

            nameRow = this.NewRow();
            nameRow[this.Columns[PROPERTY_COLUMN]] = NAME_ROW;

            plantRow = this.NewRow();
            plantRow[this.Columns[PROPERTY_COLUMN]] = PLANT_ROW;

            areaRow = this.NewRow();
            areaRow[this.Columns[PROPERTY_COLUMN]] = AREA_ROW;

            ipRow = this.NewRow();
            ipRow[this.Columns[PROPERTY_COLUMN]] = IP_ROW;

            dspRow = this.NewRow();
            dspRow[this.Columns[PROPERTY_COLUMN]] = DSP_ROW;

            this.Rows.Add(nameRow);
            this.Rows.Add(plantRow);
            this.Rows.Add(areaRow);
            this.Rows.Add(ipRow);
            this.Rows.Add(dspRow);
        }

        public LaserEquipment Laser
        {
            set
            {
                this.nameRow[this.Columns[VALUES_COLUMN]] = value.Name;
                this.plantRow[this.Columns[VALUES_COLUMN]] = value.Plant;
                this.areaRow[this.Columns[VALUES_COLUMN]] = value.Area;
                this.ipRow[this.Columns[VALUES_COLUMN]] = value.Ip;
                this.dspRow[this.Columns[VALUES_COLUMN]] = value.Dsp;
            }
        }
    }
}
