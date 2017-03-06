using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalSettingsManagerLib
{
    public class LocalSettingsManagerField
    {
        public string fieldName = String.Empty;
        public string value = String.Empty;
        public LocalSettingsManagerField(string fieldName, string value)
        {
            this.fieldName = fieldName;
            this.value = value;
        }
    }
}
