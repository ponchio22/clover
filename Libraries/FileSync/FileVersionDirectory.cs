using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace Valutech.FileSync
{
    public static class FileVersionDirectory
    {
        public static string Suffix = "_Directory.xml";

        public static string RootNode = "Directory";

        public static string FileNode = "File";

        public static string NameAttribute = "Name";

        public static string LastWriteAttribute = "LastWriteTime";

        public static string FileSizeAttribute = "FileSize";

        /// <summary>
        /// How many levels to go into the directories (including the root)
        /// 1 = Just the Root
        /// </summary>
        private static int depth = 1;

        /// <summary>
        /// Gets the directory xml file path
        /// </summary>
        /// <param name="path">Directory Path</param>
        public static string GetFilePath(string path)
        {
            Regex reg = new Regex(@"[^\\]{1,}$");
            string dn = Path.Combine(path);
            string directoryName = reg.Matches(dn)[0].ToString().Replace(@"\", String.Empty);
            string directoryFileName = Path.Combine(path, directoryName + FileVersionDirectory.Suffix);
            return directoryFileName;
        }

        /// <summary>
        /// Method to create the directory list of the supplied folder
        /// </summary>
        /// <param name="path">Root path of all the profiles to keep in sync</param>
        public static void Create(string path)
        {
            Create(path, false, null,0);
        }

        /// <summary>
        /// Method to create the directory list of the supplied folder and the levels indicated with the depth variable
        /// </summary>
        /// <param name="path">Root path of all the profiles to keep in sync</param>
        /// <param name="depth">Depth of the directory list creation</param>
        public static void Create(string path, int depth)
        {
            FileVersionDirectory.depth = depth;
            Create(path);
        }

        /// <summary>
        /// Recursive method to create the directory of all the files in the root
        /// </summary>
        /// <param name="path">Path of the folder to</param>
        /// <param name="onlyAdd">True if is going to only add the files to the writer parameter</param>
        /// <param name="writer">Writer to use in case only add is true</param>
        /// <param name="level">Depth of the current directory</param>
        private static void Create(string path, bool onlyAdd,XmlWriter writer,int level)
        {
            if (Directory.Exists(path))
            {                
                string directoryFileName = GetFilePath(path);                
                try
                  {

                    // Create an XmlWriterSettings object with the correct options. 
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ("\t");
                    settings.OmitXmlDeclaration = true;
                    if (File.Exists(directoryFileName)) File.Delete(directoryFileName);
                    string[] files = Directory.GetFiles(path);
                    if (!onlyAdd)
                    {
                        writer = XmlWriter.Create(directoryFileName, settings);
                        writer.WriteStartElement(RootNode);
                    }
                    foreach (string file in files)
                    {                        
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Extension != String.Empty)
                        {
                            writer.WriteStartElement(FileNode);
                            writer.WriteAttributeString(LastWriteAttribute, File.GetLastWriteTime(file).ToString());
                            writer.WriteAttributeString(FileSizeAttribute, fileInfo.Length.ToString());
                            writer.WriteString(file);
                            writer.WriteEndElement();
                        }
                    }
                    string[] directories = Directory.GetDirectories(path);
                    level++;
                    foreach (string directory in directories)
                    {                        
                        Create(directory, true, writer,level);
                        if (level < depth)
                        {
                            Create(directory,false,null,level);
                        }                        
                    }
                    if (!onlyAdd)
                    {
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                }
                finally
                {
                    if (!onlyAdd)
                    {
                        if (writer != null)
                        {
                            writer.Close();
                            FileVersionHandler.CreateVersion(directoryFileName);
                        }
                    }
                }
            }            
        }
                
    }
}
