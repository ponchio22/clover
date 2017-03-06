using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroxProgramsManagmentLib
{
    public class ElectroxProgramManagment
    {
        private static ElectroxProgramManagment instance;

        private List<ElectroxProgramInfo> programs = new List<ElectroxProgramInfo>();

        private ElectroxProgramManagment()
        {
            LoadData();
        }

        public static ElectroxProgramManagment GetInstance() {
            if (instance == null) instance = new ElectroxProgramManagment();
            return instance;
        }

        public bool LoadData()
        {
            programs.Clear();
            programs.Add(new ElectroxProgramInfo(1, "IPHONE5","iPhone 5 Logo"));
            programs.Add(new ElectroxProgramInfo(2, "IPHONE5S", "iPhone 5S Logo"));
            return true;
        }

        public ElectroxProgramInfo GetProgramInfoFromId(int id)
        {
            foreach (ElectroxProgramInfo program in programs)
            {
                if (program.Id == id)
                {
                    return program;
                }
            }
            return null;
        }
    }
}
