using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valutech.Electrox
{
    public class LaserProgram
    {
        private string name = string.Empty;
        private string size = string.Empty;

        public LaserProgram(string name, string size)
        {
            this.name = name;
            this.size = size;
        }

        public LaserProgram()
        {   
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Size
        {
            get
            {
                return this.size;
            }
        }

        public LaserProgram ProcessLaserOutputString(string output)
        {
            string[] splitted = output.Split(' ');
            this.name = splitted[0];
            this.size = splitted[1];
            return this;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
