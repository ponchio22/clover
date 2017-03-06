using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Valutech.IO
{
    public delegate void FileNameChangedEventHandler(string fileName);

    public class Log
    {
        private static Log instance;

        private string logFileName = null;

        public event FileNameChangedEventHandler FileNameChanged;

        public Log()
        {
            try
            {
                if (logFileName != null)
                {
                    if (!File.Exists(logFileName)) File.Create(logFileName).Close();                    
                }
            }
            catch
            {
                Console.WriteLine("Unable to create log " + logFileName);
            }
        }

        public static Log GetInstance() {
            if (instance == null) Log.instance = new Log();            
            return Log.instance;
        }

        public void SetEntry(String entry)
        {
            try
            {
                if (logFileName != null)
                {
                    if (!File.Exists(logFileName)) File.Create(logFileName).Close();
                    if (File.Exists(logFileName))
                    {
                        using (StreamWriter writer = new StreamWriter(logFileName, true))
                        {
                            writer.WriteLine(string.Format("{0:yyyy/MM/dd hh:mm:sstt}", DateTime.Now) + "   " + entry);
                            writer.Close();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Sets the filename (deprecated)
        /// </summary>
        /// <param name="filename"></param>
        public void SetFileName(String filename)
        {
            FileName = filename;
        }

        /// <summary>
        /// Getter / Setter for the filename
        /// </summary>
        public string FileName
        {
            get
            {
                return logFileName;
            }
            set
            {
                if (value != logFileName)
                {
                    logFileName = value;
                    if (FileNameChanged != null) FileNameChanged(logFileName);
                }
            }
        }
    }
}
