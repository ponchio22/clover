using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace Valutech.Electrox
{
    class LaserEquipmentListDataTable:DataTable
    {
        private LaserEquipmentListManager manager = LaserEquipmentListManager.GetInstance();

        #region Constants

        private const string NAME_COLUMN = "Name";
        private const string PLANT_COLUMN = "Plant";
        private const string IP_COLUMN = "Ip";
        private const string STATUS_COLUMN = "Status";

        #endregion


        private string plant = String.Empty;

        private string area = String.Empty;

        public LaserEquipmentListDataTable()
        {
            this.Columns.Add(NAME_COLUMN, typeof(string));
            this.Columns.Add(PLANT_COLUMN, typeof(string));
            this.Columns.Add(IP_COLUMN, typeof(string));            
        }

        public void Refresh()
        {
            List<LaserEquipment> list = manager.LaserEquipmentList;
            int i = 0;
            foreach (LaserEquipment item in list)
            {
                if ((plant == String.Empty && area == String.Empty) ||
                    (plant == String.Empty && area == item.Area) ||
                    (plant == item.Plant && area == String.Empty) ||
                    (plant == item.Plant && area == item.Area))
                {
                    DataRow row;         
                    if (i < this.Rows.Count)
                    {
                        row = this.Rows[i];
                    }
                    else
                    {
                        row = this.NewRow(); 
                        this.Rows.Add(row);
                    }
                    row[NAME_COLUMN] = item.Name;
                    row[PLANT_COLUMN] = item.Plant;
                    row[IP_COLUMN] = item.Ip;
                    i++;
                }                
            }
            if (i < this.Rows.Count)
            {
                int count = this.Rows.Count;
                for (int j = 0; j < count - i; j++)
                {
                    this.Rows[this.Rows.Count-1].Delete();
                }
            }
        }

        #region Setters 

        /// <summary>
        /// Setter for the plantto feed in the datatable
        /// </summary>
        public string Plant
        {
            set
            {
                this.plant = value;
            }
        }

        /// <summary>
        /// Setter for the area to feed in the datatable
        /// </summary>
        public string Area
        {
            set
            {
                this.area = value;
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets the data column object of the name column
        /// </summary>
        public DataColumn NameColumn
        {
            get
            {
                return this.Columns[NAME_COLUMN];
            }
        }

        /// <summary>
        /// Gets the data column object of the plant column
        /// </summary>
        public DataColumn PlantColumn
        {
            get
            {
                return this.Columns[PLANT_COLUMN];
            }
        }

        /// <summary>
        /// Gets the data column object of the ip column
        /// </summary>
        public DataColumn IpColumn
        {
            get
            {
                return this.Columns[IP_COLUMN];
            }
        }

        /// <summary>
        /// Gets the data column object of the status column
        /// </summary>
        public DataColumn StatusColumn
        {
            get
            {
                return this.Columns[STATUS_COLUMN];
            }
        }

        #endregion

    }
}
