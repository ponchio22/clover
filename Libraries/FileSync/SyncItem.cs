using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Diagnostics;
using Valutech.IO;
using System.Net;
using System.IO;

namespace Valutech.FileSync
{    
    public delegate void SyncItemEventHandler(object sender, SyncItemEventArgs args);

    #region Event Arguments 
    /// <summary>
    /// Custom class for the arguments of the filesync activity event
    /// </summary>
    public class SyncItemEventArgs : EventArgs
    {
        private string _file;
        private DateTime _datetime;
        public SyncItemEventArgs(string file)
        {
            this.file = file;
        }
        public string file
        {
            set
            {
                this._file = value;
                this._datetime = DateTime.Now;
            }
            get
            {
                return this._file;
            }
        }
        public DateTime datetime
        {
            get
            {
                return this._datetime;
            }
        }
    }

    #endregion

    /// <summary>
    /// Activity for all the file sync operations
    /// </summary>
    public class SyncItem
    {
        private String _name;

        private String _origin;

        private String _destiny;

        private string syncdirectory;

        private XDocument Xdocument;

        #region Events

        public event SyncItemEventHandler FileUpdated;

        public event SyncItemEventHandler FileCreated;

        public event SyncItemEventHandler FileDeleted;

        public event SyncItemEventHandler DirectoryCreated;

        public event SyncItemEventHandler DirectoryDeleted;

        #endregion

        public SyncItem(String name, String origin, String destiny)
        {
            this._name = name;
            this._origin = origin;
            this._destiny = destiny;
        }

        public String name
        {
            get { return this._name; }
        }

        public String origin
        {
            get { return this._origin; }
        }

        public String destiny
        {
            get { return this._destiny; }
        }

        public string SyncDirectory
        {
            set
            {
                this.syncdirectory = value;
            }
            get
            {
                return this.syncdirectory;
            }
        }
        /// <summary>
        /// Syncs the online origin path to the local machine
        /// </summary>
        public void Sync()
        {
            string destinyFile= String.Empty;
            IEnumerable<XElement> fileEntries = null;
            bool directoryNotFoundInList = false;
            //Search for origin path in the directory list and load the files if found
            try
            {
                Xdocument = XDocument.Load(syncdirectory);
                fileEntries = (from file in Xdocument.Descendants(FileVersionDirectory.FileNode)
                               where file.Value.Contains(this.origin)
                               select file);
            }
            catch (Exception)
            {
                directoryNotFoundInList = true;
            }
            if (!directoryNotFoundInList)
            {
                if (fileEntries.Count() > 0)
                {
                    //Loop thru all the found files and check for missing files or different values in the local machine
                    foreach (XElement fileEntry in fileEntries)
                    {
                        try
                        {
                            destinyFile = fileEntry.Value.Replace(this.origin, this.destiny);
                            string destinyPath = Path.GetDirectoryName(destinyFile);
                            if (!Directory.Exists(destinyPath))
                            {
                                Directory.CreateDirectory(destinyPath);
                                Console.WriteLine("Directory created: " + destinyPath);
                                if (DirectoryCreated != null) DirectoryCreated(this, new SyncItemEventArgs(destinyPath));
                                
                            }
                            FileInfo file = new FileInfo(destinyFile);
                            if (!File.Exists(destinyFile))
                            {
                                File.Copy(fileEntry.Value, destinyFile, true);
                                Console.WriteLine("File created: " + destinyFile);
                                if (FileCreated != null) FileCreated(this,new SyncItemEventArgs(destinyFile));
                            }
                            else if (file.LastWriteTime.ToString() != fileEntry.Attribute(FileVersionDirectory.LastWriteAttribute).Value)
                            {
                                File.Copy(fileEntry.Value, destinyFile, true);
                                Console.WriteLine("File updated: " + destinyFile);
                                if (FileUpdated != null) FileUpdated(this, new SyncItemEventArgs(destinyFile));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.GetInstance().SetEntry(ex.ToString());
                        }
                    }


                    //Search in the local machine for extra files and remove them if not found in the directory
                    if (!isFile(destiny))
                    {
                        DeleteFolderExtraFiles(destiny);
                    }
                }
                else
                {     
                    //If the file was not found force the copy
                    string rootPath = System.IO.Path.GetPathRoot(this.origin); // get drive's letter
                    System.IO.DriveInfo driveInfo = new System.IO.DriveInfo(rootPath); // get info about the drive                
                    if (((isFile(this.origin) && File.Exists(this.origin))||(!isFile(this.origin) && Directory.Exists(this.origin))) && !(driveInfo.DriveType == DriveType.Network))
                    {
                        if (this.isFile(this.origin))
                        {
                            SyncLocalFile(this.origin, this.destiny);
                        }
                        else
                        {
                            SyncLocalDirectory(this.origin, this.destiny);
                        }
                    }
                }
            }
        }

        private void SyncLocalDirectory(string path,string destiny)
        {
            if (Directory.Exists(path))
            {
                //Sync directories
                if (!Directory.Exists(destiny))
                {
                    Directory.CreateDirectory(destiny);
                }                
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {                    
                    string dirName = directory.Replace(path + "\\", string.Empty);
                    string dest = Path.Combine(destiny, dirName);
                    SyncLocalDirectory(directory, dest);
                }
                //Sync files
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    SyncLocalFile(file,Path.Combine(destiny,Path.GetFileName(file)));
                }
                //Delete extra files from folder
                string[] destinyfiles = Directory.GetFiles(destiny);
                foreach (string destinyfile in destinyfiles)
                {
                    string f = Path.Combine(path, Path.GetFileName(destinyfile));
                    if (!File.Exists(f))
                    {
                        File.Delete(destinyfile);
                        Console.WriteLine("File deleted: " + destinyfile);
                    }
                }
                //Delete extra directories
                string[] destinyDirectories = Directory.GetDirectories(destiny);
                foreach (string destinyDirectory in destinyDirectories)
                {
                    string d = Path.Combine(path, destinyDirectory.Replace(Path.GetDirectoryName(destinyDirectory) + "\\", String.Empty));
                    if (!Directory.Exists(d))
                    {
                        try
                        {
                            Directory.Delete(destinyDirectory, true);
                            Console.WriteLine("Directory Deleted: " + destinyDirectory);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void SyncLocalFile(string origin,string destiny)
        {
            if (!File.Exists(destiny) || (File.Exists(destiny) && (new FileInfo(destiny).LastWriteTime.ToString() != new FileInfo(origin).LastWriteTime.ToString())))
            {
                try
                {
                    File.Copy(origin, destiny, true);
                    Console.WriteLine("File created or updated: " + destiny);
                }
                catch
                {
                }
            }
        }

        public void DeleteFolderExtraFiles(string path)
        {
            Xdocument = XDocument.Load(syncdirectory);
            string[] files = Directory.GetFiles(path);
            string onlineFile = String.Empty;
            string onlineDirectory = String.Empty;
            foreach (string file in files)
            {
                try
                {
                    onlineFile = file.Replace(destiny, origin);
                    XElement fileEntries = (from fileInDir in Xdocument.Descendants(FileVersionDirectory.FileNode)
                                            where fileInDir.Value.Contains(onlineFile)
                                            select fileInDir).Single();
                }
                catch(InvalidOperationException)
                {
                    File.Delete(file);
                    Console.WriteLine("File deleted: {0}",file);
                }                
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                try
                {
                    onlineDirectory = directory.Replace(destiny, origin);
                    IEnumerable<XElement> fileEntries = (from fileInDir in  Xdocument.Descendants(FileVersionDirectory.FileNode)
                                                         where fileInDir.Value.Contains(onlineDirectory)
                                                         select fileInDir);
                    DeleteFolderExtraFiles(directory);
                    if (fileEntries.Count() == 0)
                    {                        
                        Directory.Delete(directory);
                    }                    
                }
                catch
                {
                }
            }
        }

        public Boolean isFile(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(@path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return false;
                else
                    return true;
            }
            catch
            {
                return true;
            }
        }
        
    }
}
