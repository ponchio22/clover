using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace Valutech.FileSync
{
    public static class FileVersionHandler
    {
        public static string CreateVersion(string filepath)
        {
            string filename;
            using (var md5 = MD5.Create())
            {
                filename = Path.GetFileNameWithoutExtension(filepath) + "_" + BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(filepath))).Replace("-", "");
                FileStream file = null;
                try
                {
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(filepath));
                    foreach (string ifile in files)
                    {
                        if (Path.GetExtension(ifile) == "")
                        {
                            File.Delete(ifile);
                        }
                    }
                    file = File.Create(Path.Combine(Path.GetDirectoryName(filepath), filename));
                }
                catch
                {

                }
                if (file != null) file.Close();
                return Path.Combine(Path.GetDirectoryName(filepath), filename);
            }
        }

        public static void DeleteVersion(string filepath)
        {

            using (var md5 = MD5.Create())
            {
                try
                {                    
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(filepath));
                    foreach (string ifile in files)
                    {
                        if (Path.GetExtension(ifile) == "")
                        {
                            File.Delete(ifile);
                        }
                    }                    
                }
                catch
                {

                }                
            }     
        }

        public static string GetVersion(string filepath)
        {
            using (var md5 = MD5.Create())
            {
                string filename = Path.GetFileNameWithoutExtension(filepath) + "_" + BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(filepath))).Replace("-", "");
                return filename;
            }
        }

        /// <summary>
        /// Compare the given file md5 Hash
        /// </summary>
        /// <param name="filePathLocal">Local FilePath (Will read and generate the md5 hash directly from file)</param>
        /// <param name="filePathRemote">Remote Filepath</param>
        /// <returns>Returns false if the file version is different</returns>
        public static bool CompareVersions(string filePathLocal, string filePathRemote)
        {            
            if (File.Exists(filePathLocal) && File.Exists(filePathRemote))
            {
                string localFileVersion = FileVersionHandler.GetVersion(filePathLocal);
                string localFilePath = Path.Combine(Path.GetDirectoryName(filePathLocal), localFileVersion);
                string onlineFilePath = Path.Combine(Path.GetDirectoryName(filePathRemote), localFileVersion);
                if (!File.Exists(onlineFilePath))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
