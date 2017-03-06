using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Valutech.Agilent
{
    public class Model
    {
        private string path;

        private OEM oem;

        private static string extension = ".xml";

        public static string PATH_EFFECT_TABLE = "Path_Effect_Table";
                
        public Model(string path, OEM oem)
        {
            this.path = path;
            this.oem = oem;
        }

        public String Path
        {
            get
            {
                return this.path;
            }
        }

        public String InUsePath
        {
            get
            {
                string innerPath = this.path;
                innerPath = innerPath.Replace(this.oem.ArchivedPath, this.oem.InUsePath);
                return innerPath;
            }
        }

        public OEM OEM
        {
            get
            {
                return this.oem.Version.GetOEM(oem.Name);
            }
        }

        public static string Extension
        {
            get
            {
                return Model.extension;
            }
        }

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.path);
            }
        }

        public string ExtraPath
        {
            get
            {
                string _extraPath = string.Empty;
                string _path = System.IO.Path.GetDirectoryName(this.path);
                if ( _path != oem.ModelsPath)
                {
                    _extraPath = _path.Replace(oem.ModelsPath + "\\",string.Empty);
                }                
                return _extraPath;
            }
        }

        public override string ToString()
        {
            string extraPath = ExtraPath;
            string suffix = string.Empty;
            if (extraPath != string.Empty) suffix = " (" + extraPath + ")";
            return Name + suffix;
        }
        
    }
}
