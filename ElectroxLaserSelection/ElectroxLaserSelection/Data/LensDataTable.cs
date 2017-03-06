using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Valutech.Electrox.Data
{
    public class LensDataTable : DataTable
    {
        public const string LENS_COLUMN = "Lens";

        public LensDataTable()
        {
            this.Columns.Add(LENS_COLUMN);
            this.Rows.Add("S163");
            this.Rows.Add("S254");            
        }
    }
}
