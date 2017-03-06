using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Valutech.Files
{
    public class FileRepresentation
    {
        private string _path;

        private string _filename;

        private string _basename;

        private string _extension;

        protected event EventHandler FileChange;

        public FileRepresentation(string path)
        {
            this._path = path;
            this.GetFileProperties();
        }

        public void GetFileProperties()
        {            
            this._filename = new Regex("[^\\\\]{1,}$").Match(_path).ToString();
            this._basename = new Regex("^[^.]{1,}").Match(_filename).ToString();
            this._extension = _filename.Replace(_basename, string.Empty);
        }

        public string path
        {
            set { 
                this._path = value;
                GetFileProperties();
            }
            get { return this._path; }
        }

        public string filename
        {
            get { return this._filename; }
        }

        public string basename
        {
            get { return this._basename; }
        }

        public string extension
        {
            get { return this._extension; }
        }

        public bool Exists()
        {
            return File.Exists(this._path);
        }

        public override string ToString()
        {
            return this._path;
        }

        public bool Delete()
        {
            try
            {
                if (Exists())
                {
                    File.Delete(this._path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected virtual void OnFileChange(EventArgs e) {
            if (FileChange != null) FileChange(this, e);
        }

        public bool InUse()
        {
            FileStream stream = null;

            try
            {
                FileInfo file = new FileInfo(this._path);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
