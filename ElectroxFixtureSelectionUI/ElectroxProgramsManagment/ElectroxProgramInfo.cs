using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroxProgramsManagmentLib
{
    public class ElectroxProgramInfo
    {
        private int id;
        private string name;
        private string friendlyName;

        public ElectroxProgramInfo(int id, string name,string friendlyName)
        {
            this.id = id;
            this.name = name;
            this.friendlyName = friendlyName;
        }

        public int Id
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this.friendlyName;
            }
        }
    }
}
