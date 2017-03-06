using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Valutech.Files
{
    public class DirectoryRepresentation
    {
        private String _path;

        public DirectoryRepresentation(String path)
        {
            this._path = path;
        }

        public bool Exists()
        {
            return Directory.Exists(this._path);
        }

        public String path
        {
            set { this._path = value; }
            get { return this._path; }
        }
    }
}
