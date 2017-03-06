using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valutech.Electrox
{
    class LensSetup
    {
        private string name;
        private bool selected;
        private float focalLengthString;
        private float workingDistanceString;
        private float diameterString;
        private float amplitudeString;
        private float xpincushonString;
        private float ypinchushonString;
        private float xyangleString;
        private string iniString;
        private int iniIndex;

        public LensSetup(string iniString,int iniIndex)
        {
            this.iniString = iniString;
            this.iniIndex = iniIndex;
            GetParametersFromString(iniString);
        }

        /// <summary>
        /// Gets the name of the lens
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Loads all the parameters into variables from the string
        /// </summary>
        /// <param name="iniString"></param>
        private void GetParametersFromString(string iniString)
        {
            string[] data = iniString.Split(',');
            this.name = data[0];
            this.selected = (data[1]=="S")? true:false;
            this.focalLengthString = float.Parse(data[2].Split('=')[1].Replace(" ", String.Empty));
            this.workingDistanceString = float.Parse(data[3].Split('=')[1].Replace(" ", String.Empty));
            this.diameterString = float.Parse(data[4].Split('=')[1].Replace(" ", String.Empty));
            this.amplitudeString = float.Parse(data[5].Split('=')[1].Replace(" ", String.Empty));
            this.xpincushonString = float.Parse(data[6].Split('=')[1].Replace(" ", String.Empty));
            this.ypinchushonString = float.Parse(data[7].Split('=')[1].Replace(" ", String.Empty));
            this.xyangleString = float.Parse(data[8].Split('=')[1].Replace(" ", String.Empty));
        }

        /// <summary>
        /// Returns a string with all the parameters based on the object variables
        /// </summary>
        /// <returns></returns>
        public string SetParametersToString()
        {
            string returnValue = String.Join(", ",this.name,(this.selected)? "S":"P","F= " +this.focalLengthString.ToString(),"W= " + this.workingDistanceString.ToString(),"D= " + this.diameterString.ToString(),"A= " + this.amplitudeString.ToString(),"X= " + this.xpincushonString.ToString(),"Y= " + this.ypinchushonString.ToString(), "X&Y= " + this.xyangleString.ToString());
            return returnValue;
        }

        public int IniIndex
        {
            get
            {
                return this.iniIndex;
            }
        }

        public bool Selected
        {
            set
            {
                this.selected = value;
            }
            get
            {
                return this.selected;
            }
        }

        public float LensAmplitude
        {
            set
            {
                this.amplitudeString = value;
            }
            get
            {
                return this.amplitudeString;
            }
        }

        public float XCompensation
        {
            set
            {
                this.xpincushonString = value;
            }
            get
            {
                return this.xpincushonString;
            }
        }

        public float YCompensation
        {
            set
            {
                this.ypinchushonString = value;
            }
            get
            {
                return this.ypinchushonString;
            }
        }
    }
}
