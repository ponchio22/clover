using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Valutech.Electrox.Data
{
    public class PlantsDataTable : DataTable
    {
        public const string FACILITY = "Facility";

        public PlantsDataTable()
        {
            this.Columns.Add(FACILITY);            
            this.Rows.Add("[All]");
            this.Rows.Add("Planta 1");
            this.Rows.Add("Planta 2");
        }
    }
}
