using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valutech.Wtm
{
    public enum CHANNEL_TECH
    {
        CDMA,
        WCDMA,
        GSM,
    }

    public class Channel
    {
        public string Name;

        public string RxFrequency;

        public string TxFrequency;

        public string Tech;

        public Channel(string name, string rx, string tx, string tech)
        {
            this.Name = name;
            this.RxFrequency = rx;
            this.TxFrequency = tx;
            this.Tech = tech;
        }

        public string RxFrequencyExp
        {
            get
            {
                return ConvertToExp(this.RxFrequency);
            }
        }

        public string TxFrequencyExp
        {
            get
            {
                return ConvertToExp(this.TxFrequency);
            }
        }

        private string ConvertToExp(string frequency)
        {
            if (frequency != String.Empty)
            {
                string currentE = "009";
                double result = Convert.ToDouble(frequency)/1000;
                if (result < 1)
                {
                    result = result * 10;
                    currentE = "008";
                }
                return result.ToString() + "E+" + currentE;
            }
            else
            {
                return String.Empty;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
