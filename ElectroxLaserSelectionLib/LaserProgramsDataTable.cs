using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace Valutech.Electrox.Data
{
    public class LaserProgramsDataTable : DataTable
    {
        public LaserProgramsDataTable()
        {
            this.Columns.Add("Programs");
        }

        public ArrayList Programs
        {
            set
            {
                this.Rows.Clear();
                foreach (LaserProgram prg in value)
                {
                    this.Rows.Add(prg.Name);
                }
            }
        }
    }
}
