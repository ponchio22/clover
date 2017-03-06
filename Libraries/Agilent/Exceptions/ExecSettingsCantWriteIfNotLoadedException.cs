using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valutech.Agilent.Exceptions
{
    /// <summary>
    /// Exception in case the wtm is not closing
    /// </summary>
    public class ExecSettingsCantWriteIfNotLoadedException : Exception
    {
        public ExecSetting ExecSetting;
        public ExecSettingsCantWriteIfNotLoadedException(ExecSetting execSetting)
            : base()
        {
            this.ExecSetting = execSetting;
        }
    }
}
