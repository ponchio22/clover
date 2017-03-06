using System;
using System.Collections.Generic;
using System.Text;

namespace Valutech.IO
{
    public class SerialDataSentEventArgs
    {
        public string Text;

        public static SerialDataSentEventArgs Empty
        {
            get
            {
                return new SerialDataSentEventArgs();
            }
        }
    }
}
