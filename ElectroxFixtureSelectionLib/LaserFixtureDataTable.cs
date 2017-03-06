using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ElectroxProgramsManagmentLib;

namespace ElectroxFixtureSelectionLib
{
    public class LaserFixtureDataTable:DataTable
    {

        #region Constants

        public const string PROPERTY_COLUMN = "Properties";

        public const string VALUES_COLUMN = "Values";

        public const string COMM_ROW = "Comm Port";

        public const string STATUS_ROW = "Status";

        public const string CODE_ROW = "Fixture";

        #endregion

        public DataRow commRow;

        public DataRow statusRow;

        public DataRow codeRow;

        private ElectroxProgramManagment programsManagment = ElectroxProgramManagment.GetInstance();

        public LaserFixtureDataTable()
        {
            this.Columns.Add(PROPERTY_COLUMN);
            this.Columns.Add(VALUES_COLUMN);

            commRow = this.NewRow();
            commRow[this.Columns[PROPERTY_COLUMN]] = COMM_ROW;

            statusRow = this.NewRow();
            statusRow[this.Columns[PROPERTY_COLUMN]] = STATUS_ROW;

            codeRow = this.NewRow();
            codeRow[this.Columns[PROPERTY_COLUMN]] = CODE_ROW;

            this.Rows.Add(commRow);
            this.Rows.Add(statusRow);
            this.Rows.Add(codeRow);
        }

        public string CommPort
        {
            set
            {
                this.commRow[this.Columns[VALUES_COLUMN]] = value;
            }
        }

        public bool Connected
        {
            set
            {
                this.statusRow[this.Columns[VALUES_COLUMN]] = (value) ? "Connected" : "Disconnected";
            }
        }

        public int Fixture
        {
            set
            {
                string outputString = string.Empty;
                ElectroxProgramInfo program = programsManagment.GetProgramInfoFromId(value);
                if (program != null)
                {
                    outputString = program.FriendlyName;
                }
                else
                {
                    outputString = "No Fixture Found";
                }
                this.codeRow[this.Columns[VALUES_COLUMN]] = outputString;
            }
        }
    }
}
