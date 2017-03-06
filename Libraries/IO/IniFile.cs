using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace Valutech.IO
{
    public class INIFile
    {

        protected string filePath;

        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
        string key,
        string val,
        string filePath);
        
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
        string key,
        string def,
        StringBuilder retVal,
        int size,
        string filePath);
        
        // Second Method
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key,
               string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result,
               int Size, string FileName);

        // Third Method
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key,
               string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result,
               int Size, string FileName);
        

        public INIFile(string filePath)
        {
            this.filePath = filePath;            
        }

        public long Write(string section, string key, string value)
        {            
            return WritePrivateProfileString(section, key, value, this.filePath);
        }

        public string Read(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, this.filePath);            
            return SB.ToString();           
        }

        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }

        // The Function called to obtain the SectionHeaders,
        // and returns them in an Dynamic Array.
        public ArrayList GetSectionNames()
        {
            ArrayList sections = new ArrayList();
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time.
            for (int maxsize = 500; true; maxsize *= 2)
            {
                //    Obtains the information in bytes and stores
                //    them in the maxsize buffer (Bytes array)
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, this.filePath);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < maxsize - 2)
                {
                    // Converts the bytes value into an ASCII char. This is one long string.
                    string Selected = Encoding.ASCII.GetString(bytes, 0,
                                               size - (size > 0 ? 1 : 0));
                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    string[] parts = Selected.Split(new char[] { '\0' });
                    foreach (string part in parts)
                    {
                        sections.Add(part);
                    }
                    return sections;
                }
            }            
        }

        public bool Exists()
        {
            try
            {
                return File.Exists(this.FilePath);
            }
            catch
            {
                return false;
            }
        }

        public void Delete()
        {
            try
            {
                File.Delete(this.FilePath);
            }
            catch
            {
            }
        }
    }
}


