using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using Valutech.Agilent.Exceptions;

namespace Valutech.Agilent
{
    public delegate void OEMPathChangedEventHandler(OEM oem);
    
    /// <summary>
    /// Represents an OEM in the wireless test manager, that means a folder with the name of the version and the oem name if not in use
    /// </summary>
    public class OEM
    {

        #region Constants

        /// <summary>
        /// Initial oem name for the object when the name has not been set
        /// </summary>
        private static string OEM_NAME_NOT_SET = "OEM NAME NOT SET";

        /// <summary>
        /// Models directory name
        /// </summary>
        public static string MODELS_DIRECTORY = "TestData";

        #endregion

        #region Objects

        /// <summary>
        /// Version the oem belongs to
        /// </summary>
        private WTMVersion version;

        /// <summary>
        /// Current path of the oem folder
        /// </summary>
        private string currentPath;

        /// <summary>
        /// Config file which contains the name of the oem, and test plan information
        /// </summary>
        private OEMConfigFile configFile;

        /// <summary>
        /// Name of the oem, retrieved originally from the folder name and if its in use from the config file
        /// </summary>
        private string name = OEM.OEM_NAME_NOT_SET;

        /// <summary>
        /// Exce Settings file, includes all the configuration parameters for the OEM
        /// </summary>
        private ExecSetting execSetting;

        #endregion

        public event OEMPathChangedEventHandler Archived;

        public event OEMPathChangedEventHandler Used;

        private ArrayList models =  new ArrayList();

        #region Exceptions

        /// <summary>
        /// Exception in case the oem couldnt be archived
        /// </summary>
        public class UnableToArchiveOEMException : Exception
        {
            public OEM OEM;
            public ErrorType Error;
            public enum ErrorType
            {
                IO_EXCEPTION,
                UNAUTHORIZED_ACCESS_EXCEPTION,
                ARCHIVED_OEM_ALREADY_EXISTS
            }
            public UnableToArchiveOEMException(OEM oem, ErrorType error)
                : base()
            {
                this.OEM = oem;
                this.Error = error;
            }
        }

        /// <summary>
        /// Exception in case the oem couldnt be used
        /// </summary>
        public class UnableToUseOEMException : Exception
        {
            public OEM OEM;
            public ErrorType Error;
            public enum ErrorType
            {
                IO_EXCEPTION,
                UNAUTHORIZED_ACCESS_EXCEPTION,
                OEM_ALREADY_IN_USE
            }
            public UnableToUseOEMException(OEM oem, ErrorType error)
                : base()
            {
                this.OEM = oem;
                this.Error = error;
            }
        }

        #endregion

        public OEM(string path, WTMVersion version)
        {
            this.setPath(path);
            this.version = version;
            this.execSetting = new ExecSetting(this);
            LoadNameFromPath();
        }


        #region General Properties

        /// <summary>
        /// Validates the oem folder to see if it has the testplan and testplan version values set
        /// </summary>
        /// <param name="oem">OEM to validate</param>
        /// <returns></returns>
        public bool Validate()
        {
            if (this.TestPlan != OEMConfigFile.NOT_SET)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use the given oem, makes the necessary folder name changes
        /// </summary>
        /// <param name="oemToUse">OEM to use</param>.
        /// <returns>Returns the WTM Version that was closed during the process if any</returns>
        /// <exception cref="Valutech.Agilent.UnableToCloseWTMException">Thrown when the wtm was not able to get closed by the system</exception>
        /// <exception cref="Valutech.Agilent.UnableToArchiveOEMException">Thrown when the oem was not able to get archived</exception>
        /// <exception cref="Valutech.Agilent.UnableToUseOEMException">Thrown when the oem was not able to get used</exception>
        /// <exception cref="Valutech.Agilent.UnableToCreateExecSettingBackupException">Thrown when the ExecSetting backup file could not be created</exception>
        public WTMVersion Use()
        {
            this.ExecSetting.Load();
            WTMVersion closedVersion = null;
            //If the oem is not in use currently we need to close the wtm and rename the folders
            WTMVersion runningVersion = WirelessTestManager.GetInstance().IsRunning();
            if (!this.InUse)
            {
                //Close the currently running wtm if running                
                if (runningVersion != null)
                {
                    if (!runningVersion.Close())
                    {
                        throw new UnableToCloseWTMException(runningVersion);
                    }
                    else
                    {
                        closedVersion = runningVersion;
                    }
                }

                //Utilizar el oem seleccionado
                if (this.Path != this.InUsePath)
                {
                    try
                    {
                        if (!Directory.Exists(this.InUsePath))
                        {
                            Directory.Move(this.Path, this.InUsePath);
                            this.setPath(InUsePath);
                            if (Used != null) Used(this);
                        }
                        else
                        {
                            throw new UnableToUseOEMException(this, UnableToUseOEMException.ErrorType.OEM_ALREADY_IN_USE);
                        }
                    }
                    catch (IOException)
                    {
                        throw new UnableToUseOEMException(this, UnableToUseOEMException.ErrorType.IO_EXCEPTION);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new UnableToUseOEMException(this, UnableToUseOEMException.ErrorType.UNAUTHORIZED_ACCESS_EXCEPTION);
                    }
                }
            }

            //Set the location of the new model in the exec settings file of the current oem in use
            

            //Close the currently running wtm if running                
            if (runningVersion != null && this.ExecSetting.NeedsRestart)
            {
                if (!runningVersion.Close())
                {
                    throw new UnableToCloseWTMException(runningVersion);
                }
                else
                {
                    closedVersion = runningVersion;
                }
            }            
            return closedVersion;
        }

        /// <summary>
        /// Use the given oem, makes the necessary folder name changes
        /// </summary>
        /// <param name="oemToUse">OEM to use</param>.
        /// <returns>Returns the WTM Version that was closed during the process if any</returns>
        /// <exception cref="Valutech.Agilent.UnableToCloseWTMException">Thrown when the wtm was not able to get closed by the system</exception>
        /// <exception cref="Valutech.Agilent.UnableToArchiveOEMException">Thrown when the oem was not able to get archived</exception>
        /// <exception cref="Valutech.Agilent.UnableToUseOEMException">Thrown when the oem was not able to get used</exception>
        /// <exception cref="Valutech.Agilent.UnableToCreateExecSettingBackupException">Thrown when the ExecSetting backup file could not be created</exception>
        public WTMVersion Archive()
        {
            WTMVersion closedVersion = null;
            //If the oem is not in use currently we need to close the wtm and rename the folders
            WTMVersion runningVersion = WirelessTestManager.GetInstance().IsRunning();            
            if (this.InUse)
            {
                //Close the currently running wtm if running                
                if (runningVersion != null)
                {
                    if (!runningVersion.Close())
                    {
                        throw new UnableToCloseWTMException(runningVersion);
                    }
                    else
                    {
                        closedVersion = runningVersion;
                    }
                }

                //Archive the oem in use
                if (this != null)
                {
                    if (this.Path != this.ArchivedPath)
                    {
                        try
                        {
                            if (!Directory.Exists(this.ArchivedPath))
                            {
                                Directory.Move(this.Path, this.ArchivedPath);
                                this.setPath(ArchivedPath);
                                if (Archived != null) Archived(this);
                            }
                            else
                            {
                                throw new UnableToArchiveOEMException(this, UnableToArchiveOEMException.ErrorType.ARCHIVED_OEM_ALREADY_EXISTS);
                            }
                        }
                        catch (IOException)
                        {
                            throw new UnableToArchiveOEMException(this, UnableToArchiveOEMException.ErrorType.IO_EXCEPTION);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            throw new UnableToArchiveOEMException(this, UnableToArchiveOEMException.ErrorType.UNAUTHORIZED_ACCESS_EXCEPTION);
                        }
                    }
                }
            }

            //Close the currently running wtm if running                
            if (runningVersion != null && this.ExecSetting.NeedsRestart)
            {
                if (!runningVersion.Close())
                {
                    throw new UnableToCloseWTMException(runningVersion);
                }
                else
                {
                    closedVersion = runningVersion;
                }
            }
            return closedVersion;
        }

        #endregion

        public ExecSetting ExecSetting
        {
            get
            {
                return this.execSetting;
            }
        }

        public string Path
        {
            get
            {
                return this.currentPath;
            }
        }

        private void setPath(string value) {          
            this.currentPath = value;             
            this.configFile = new OEMConfigFile(this);
            this.LoadModels();
        }

        public string ArchivedPath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), Version.Name + name);
            }
        }

        public string InUsePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), Version.Name);
            }
        }

        public string ModelsPath
        {
            get
            {
                return System.IO.Path.Combine(this.Path, MODELS_DIRECTORY);
            }
        }

        public WTMVersion Version
        {
            get
            {
                return WirelessTestManager.GetInstance().GetVersion(this.version.Name);
            }
        }

        public string Name
        {
            set
            {
                if (!this.InUse)
                {
                    this.name = value;
                    this.configFile.brand = this.name;
                    string wtmpath = WirelessTestManager.GetInstance().GetPath();
                    string newPath = System.IO.Path.Combine(wtmpath, version.Name + name);
                    Directory.Move(this.Path, newPath);
                    this.setPath(newPath);
                }
                else
                {
                    this.name = value;
                    this.configFile.brand = this.name;
                }
            }
            get
            {                
                return this.name;
            }
        }

        public override string ToString()
        {
            return this.name;
        }

        private void LoadNameFromPath()
        {
            Regex regexp = new Regex(".{0,}E[0-9]{4}[a-zA-Z]{1}");
            String retName = regexp.Replace(this.Path, "");
            this.configFile = new OEMConfigFile(this);
            if (retName != string.Empty) configFile.brand = retName;
            this.name = configFile.brand;
        }

        public string TestPlan
        {
            get
            {
                return configFile.testPlan;
            }
            set
            {
                configFile.testPlan = value;
            }
        }

        public string TestPlanVersion
        {
            get
            {
                return configFile.testPlanVersion;
            }
            set
            {
                configFile.testPlanVersion = value;
            }
        }

        public bool InUse
        {
            get
            {
                if (this.version.Name + this.name == System.IO.Path.GetFileName(this.Path))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #region Models Managing

        /// <summary>
        /// Gets the models found inside the oem folder
        /// </summary>
        /// <param name="oem"></param>
        /// <returns></returns>
        public ArrayList GetModels()
        {
            if(models.Count == 0) models = GetModelsInFolder(ModelsPath);
            return models;
        }

        /// <summary>
        /// Load the models found inside the folder
        /// </summary>
        public void LoadModels()
        {
            models = GetModelsInFolder(ModelsPath);
        }

        /// <summary>
        /// Get the model object from the name
        /// </summary>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public Model GetModel(string modelName)
        {
            GetModels();
            foreach (Model model in models)
            {
                if (model.Name == modelName) return model;
            }
            return null;
        }

        public Model GetModelFromPath(string modelPath)
        {
            GetModels();
            foreach (Model model in models)
            {
                if (model.Path == modelPath) return model;
            }
            return null;
        }

        /// <summary>
        /// Recursive method to retrieve the files and the folders and files inside of the oem
        /// </summary>
        /// <param name="searchPath"></param>
        /// <param name="oem"></param>
        /// <returns>Returns the list of all the models found inside the path</returns>
        private ArrayList GetModelsInFolder(string searchPath)
        {
            ArrayList models = new ArrayList();
            Regex re = new Regex("([^\\\\][^\\\\]{1,}$)");
            Regex excludedRe = new Regex("MoMConfig|" + System.IO.Path.GetFileNameWithoutExtension(ExecSetting.FILENAME));
            String folderName;
            MatchCollection matches;
            String textPrefix = string.Empty;
            matches = re.Matches(searchPath);
            if (matches.Count > 0)
            {
                folderName = matches[0].Value.ToString();
                if (folderName != OEM.MODELS_DIRECTORY) textPrefix = folderName;
                //Get the files in the directory
                string[] files = Directory.GetFiles(searchPath);
                foreach (string file in files)
                {
                    string filename = System.IO.Path.GetFileName(file);
                    if (System.IO.Path.GetExtension(file).ToLower() == Model.Extension.ToLower() && !excludedRe.IsMatch(filename))
                    {
                        Model model = new Model(file, this);
                        models.Add(model);
                    }
                }
                //Get the folders inside the folder (Recursive)
                string[] dirs = Directory.GetDirectories(searchPath);
                foreach (string dir in dirs)
                {
                    ArrayList dirModels = GetModelsInFolder(dir);
                    foreach (Model model in dirModels)
                    {
                        models.Add(model);
                    }
                }
            }
            return models;
        }

        #endregion
    }
}
