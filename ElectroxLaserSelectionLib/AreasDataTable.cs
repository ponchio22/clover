using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Valutech.Electrox.Data
{
    public class AreasDataTable : DataTable
    {
        public const string AREA_COLUMN = "Area";

        public const string ALL_AREAS = "[All]";

        public const string ANODIZING_AREA = "Anodizing";

        public const string BCOVER_CENTER_AREA = "B Cover Center";

        public const string IMEI_AREA = "IMEI";

        public AreasDataTable()
        {
            this.Columns.Add(AREA_COLUMN);
            this.Rows.Add(ALL_AREAS);
            this.Rows.Add(ANODIZING_AREA);
            this.Rows.Add(BCOVER_CENTER_AREA);
            this.Rows.Add(IMEI_AREA);
        }
    }
}
