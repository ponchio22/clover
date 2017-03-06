using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valutech.Agilent.Exceptions
{
    /// <summary>
    /// Exception in case the wtm is not closing
    /// </summary>
    public class UnableToCloseWTMException : Exception
    {
        public WTMVersion Version;
        public UnableToCloseWTMException(WTMVersion version)
            : base()
        {
            this.Version = version;
        }
    }
}
