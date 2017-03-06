using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valutech.IO;

namespace Valutech.Electrox
{
    class MarkerIniFile:INIFile
    {
        private const string LENS_SETUPS_SECTION = "LENS SETUPS";

        private const string LENS_SETUPS2_SECTION = "LENS SETUPS2";

        private const string LENS_PREFIX = "LENS";

        private List<LensSetup> lensSetups = new List<LensSetup>();

        public MarkerIniFile(string file)
            : base(file)
        {
        }

        public void LoadLensSetups()
        {
            string value = string.Empty;
            int count = 1;
            do
            {
                value = Read(LENS_SETUPS_SECTION, LENS_PREFIX + count.ToString());
                if (value != string.Empty)
                {
                    lensSetups.Add(new LensSetup(value,count));
                }
                count++;
            } while (value!=string.Empty);
        }

        public void SetSetupSelected(LensSetup setup)
        {
            foreach (LensSetup lensSetup in lensSetups)
            {
                if (setup.Name == lensSetup.Name)
                {
                    lensSetup.Selected = true;
                    lensSetup.LensAmplitude = setup.LensAmplitude;
                    lensSetup.XCompensation = setup.XCompensation;
                    lensSetup.YCompensation = setup.YCompensation;
                }                
            }
            foreach (LensSetup lensSetup in lensSetups)
            {                
                Write(LENS_SETUPS_SECTION, LENS_PREFIX + lensSetup.IniIndex.ToString(), lensSetup.SetParametersToString());
                Write(LENS_SETUPS2_SECTION, LENS_PREFIX + lensSetup.IniIndex.ToString(), lensSetup.SetParametersToString());
            }
        }

        public List<LensSetup> LensSetups
        {
            get
            {
                return this.lensSetups;
            }
        }
    }
}
