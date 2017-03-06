using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Valutech.Agilent.Exceptions;

namespace Valutech.Agilent
{

    #region Custom Exceptions

    /// <summary>
    /// Thrown when its not able to create a backup of the execsetting file
    /// </summary>
    public class UnableToCreateExecSettingBackupException : Exception
    {
        public ExecSetting ExecSetting;
        public ErrorType Error;
        public enum ErrorType
        {
            IO_EXCEPTION,
            UNAUTHORIZED_ACCESS_EXCEPTION,
            FILE_NOT_FOUND,
            OTHER
        }
        public UnableToCreateExecSettingBackupException(ExecSetting ExecSetting,ErrorType error)
            : base()
        {
            this.ExecSetting = ExecSetting;
            this.Error = error;
        }
    }

    /// <summary>
    /// Thrown when its not able to restore a backup of the execsetting file
    /// </summary>
    public class UnableToRestoreExecSettingBackupException : Exception
    {
        public ExecSetting ExecSetting;
        public ErrorType Error;
        public enum ErrorType
        {
            IO_EXCEPTION,
            UNAUTHORIZED_ACCESS_EXCEPTION,
            BACKUP_NOT_FOUND,
            OTHER
        }
        public UnableToRestoreExecSettingBackupException(ExecSetting ExecSetting, ErrorType error)
            : base()
        {
            this.ExecSetting = ExecSetting;
            this.Error = error;
        }
    }

    /// <summary>
    /// Thrown when the exec setting file is not valid
    /// </summary>
    public class InvalidExecSettingFileException : Exception
    {
        public ExecSetting ExecSetting;
        public InvalidExecSettingFileException(ExecSetting ExecSetting)
        {
            this.ExecSetting = ExecSetting;
        }
    }

    #endregion

    public class ExecSetting
    {
        #region Constants

        public static string FILENAME = "ExecSetting.xml";

        public static string BACKUP_EXTENSION = ".bk";

        public static string BACKUP_FILENAME = FILENAME + BACKUP_EXTENSION;

        #endregion

        #region Objects and Variables

        /// <summary>
        /// Wireless Test Manager Object
        /// </summary>
        private WirelessTestManager wtm;

        /// <summary>
        /// OEM The ExecSettings File belongs to
        /// </summary>
        private OEM oem;

        /// <summary>
        /// Indicates if the file has been loaded to the memory
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Indicates if the changes made to the file needs to restart the wireless test manager
        /// </summary>
        private bool fileNeedsRestart = false;

        /// <summary>
        /// Indicates if the file has changes that has not been saved
        /// </summary>
        private bool changesNotSaved = false;

        #endregion 

        #region File Properties variables and Default values

        private string _password;
        private string _saveResultsToFile;
        private string _resultsFileType;
        private string _keepResultsFromAborted;
        private string _resultsFilePath;
        private string _resultsFilePathSecondary;
        private string _displayLastMeas;
        private string _pathLossFile;
        private string _printType;
        private string _database;
        private string _defineTestPlanTab;
        private string _specificationsTab;
        private string _parametersTab;
        private string _globalParametersTab;
        private string _instrumentControlTab;
        private string _barcodeReaderControlTab;
        private string _dutControlTab;
        private string _displayControlTab;
        private string _printerTab;
        private string _runTestConditionsTab;
        private string _dataCollectionTab;
        private string _pathLossViewTab;
        private string _fixtureControlTab;
        private string _systemIOPortsTab;
        private string _testPlanStartTab;
        private string _testPlanAttributes;

        public const string DEFAULT_PASSWORD = "w|bbw`wo}";
        public const string DEFAULT_SAVE_RESULTS_TO_FILE = "1";
        public const string DEFAULT_RESULTS_FILE_TYPE = "1";
        public const string DEFAULT_KEEP_RESULTS_FROM_ABORTED = "1";
        public const string DEFAULT_RESULTS_FILE_PATH = @"C:\Program Files\Agilent\WirelessTestManager\E6567C\TestResults\";
        public const string DEFAULT_RESULTS_FILE_PATH_SECONDARY = DEFAULT_RESULTS_FILE_PATH;
        public const string DEFAULT_DISPLAY_LAST_MEAS = "1";
        public const string DEFAULT_PRINT_TYPE = "0";        
        public const string DEFAULT_DEFINE_TEST_PLAN_TAB = "0";
        public const string DEFAULT_SPECIFICATIONS_TAB = "0";
        public const string DEFAULT_PARAMETERS_TAB = "0";
        public const string DEFAULT_GLOBAL_PARAMETERS_TAB = "0";
        public const string DEFAULT_INSTRUMENT_CONTROL_TAB = "0";
        public const string DEFAULT_BARCODE_READER_CONTROL_TAB = "0";
        public const string DEFAULT_DUT_CONTROL_TAB = "0";
        public const string DEFAULT_DISPLAY_CONTROL_TAB = "0";
        public const string DEFAULT_PRINTER_TAB = "1";
        public const string DEFAULT_RUN_TEST_CONDITIONS_TAB = "0";
        public const string DEFAULT_DATA_COLLECTION_TAB = "0";
        public const string DEFAULT_PATHLOSS_VIEW_TAB = "0";
        public const string DEFAULT_FIXTURE_CONTROL_TAB = "0";
        public const string DEFAULT_SYSTEM_IOPORTS_TAB = "0";
        public const string DEFAULT_TESTPLAN_START_TAB = "0";
        public const string DEFAULT_TESTPLAN_ATTRIBUTES_TAB = "0";

        #endregion

        #region XMLNodes
        /// <summary>
        /// Enum with all the needed execsettings options
        /// </summary>
        private enum ExecSettingsFileNodes : int
        {
            ExecCommon, //Tag of the xml file
            Exec1, //Tag of the xml file
            Executive_Settings, //Tag of the xml file

            Password, //Password for the system
            SaveResultsToFile, //Default Value = 1
            ResultsFileType, //Default Value = 1
            KeepResultsFromAborted, //Default Value = 1
            ResultsFilePath,
            ResultsFilePathSecondary,
            DisplayLastMeas, //Default Value = 1
            PathLossFile,
            PrintType,
            AppDataPath, //Database
            //Enable or disable the selected window
            Extended1, //Define test plan
            Extended2, //Specifications
            Extended3, //Parameters
            Extended4, //Global Parameters
            Extended5, //Instrument Control
            Extended6, //Barcode reader control
            Extended7, //DUT Control
            Extended8, //Display Control
            Extended9, //Printer
            Extended10, //Run Test Conditions
            Extended11, //Data Collection
            Extended12, //Pathloss View
            Extended13, //Fixture Control
            Extended14, //System IO Ports
            Extended15, //Testplan Start
            Extended16, //Testplan Attributes
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        public ExecSetting(OEM oem)
        {
            this.oem = oem;
            this.wtm = WirelessTestManager.GetInstance();
            this.wtm.WTMRestarted += new WTMRestartedEventHandler(wtm_WTMRestarted);
            this.wtm.WTMClosed += new WTMRestartedEventHandler(wtm_WTMRestarted);
        }

        #region General Properties

        /// <summary>
        /// Gets the path of the exec setting file
        /// </summary>
        public string Path
        {
            get
            {
                return System.IO.Path.Combine(this.oem.ModelsPath, ExecSetting.FILENAME);
            }
        }

        /// <summary>
        /// Indicates if the file has been changed and it needs the wtm to restart to take changes
        /// </summary>
        public bool NeedsRestart
        {
            get
            {
                return fileNeedsRestart;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the event when the wtm has been restarted and resets the flag to false
        /// </summary>
        /// <param name="version"></param>
        private void wtm_WTMRestarted(WTMVersion version)
        {
            fileNeedsRestart = false;
            loaded = false;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Loads the file into the variables or write the values of the variables into the file
        /// </summary>
        /// <param name="load">Indicates if its in reading or writing mode</param>
        /// <returns>Bool indicating if it successfully finish the writing or reading</returns>
        public bool LoadWrite(bool load)
        {
            if (!this.loaded && load || this.loaded && !load)
            {
                if (File.Exists(Path))
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(Path);
                        XmlNodeList xmlNodes;
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            xmlNodes = node.ChildNodes;
                            if (node.Name == ExecSettingsFileNodes.Exec1.ToString() || node.Name == ExecSettingsFileNodes.ExecCommon.ToString())
                            {
                                foreach (XmlNode innerNode in xmlNodes)
                                {
                                    if (load)
                                    {
                                        //Load the values into the variables
                                        if (innerNode.Name == ExecSettingsFileNodes.Password.ToString()) this._password = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.SaveResultsToFile.ToString()) this._saveResultsToFile = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFileType.ToString()) this._resultsFileType = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.KeepResultsFromAborted.ToString()) this._keepResultsFromAborted = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFilePath.ToString()) this._resultsFilePath = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFilePathSecondary.ToString()) this._resultsFilePathSecondary = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.DisplayLastMeas.ToString()) this._displayLastMeas = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.PathLossFile.ToString()) this._pathLossFile = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.PrintType.ToString()) this._printType = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.AppDataPath.ToString()) this._database = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended1.ToString()) this._defineTestPlanTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended2.ToString()) this._specificationsTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended3.ToString()) this._parametersTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended4.ToString()) this._globalParametersTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended5.ToString()) this._instrumentControlTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended6.ToString()) this._barcodeReaderControlTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended7.ToString()) this._dutControlTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended8.ToString()) this._displayControlTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended9.ToString()) this._printerTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended10.ToString()) this._runTestConditionsTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended11.ToString()) this._dataCollectionTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended12.ToString()) this._pathLossViewTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended13.ToString()) this._fixtureControlTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended14.ToString()) this._systemIOPortsTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended15.ToString()) this._testPlanStartTab = innerNode.InnerText;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended16.ToString()) this._testPlanAttributes = innerNode.InnerText;                                        
                                    }
                                    else
                                    {
                                        //Write the values into the xml file
                                        if (innerNode.Name == ExecSettingsFileNodes.Password.ToString()) innerNode.InnerText = this._password;
                                        if (innerNode.Name == ExecSettingsFileNodes.SaveResultsToFile.ToString()) innerNode.InnerText = this._saveResultsToFile;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFileType.ToString()) innerNode.InnerText = this._resultsFileType;
                                        if (innerNode.Name == ExecSettingsFileNodes.KeepResultsFromAborted.ToString()) innerNode.InnerText = this._keepResultsFromAborted;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFilePath.ToString()) innerNode.InnerText = this._resultsFilePath;
                                        if (innerNode.Name == ExecSettingsFileNodes.ResultsFilePathSecondary.ToString()) innerNode.InnerText = this._resultsFilePathSecondary;
                                        if (innerNode.Name == ExecSettingsFileNodes.DisplayLastMeas.ToString()) innerNode.InnerText = this._displayLastMeas;
                                        if (innerNode.Name == ExecSettingsFileNodes.PathLossFile.ToString()) innerNode.InnerText = this._pathLossFile;
                                        if (innerNode.Name == ExecSettingsFileNodes.PrintType.ToString()) innerNode.InnerText = this._printType;
                                        if (innerNode.Name == ExecSettingsFileNodes.AppDataPath.ToString()) innerNode.InnerText = this._database;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended1.ToString()) innerNode.InnerText = this._defineTestPlanTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended2.ToString()) innerNode.InnerText = this._specificationsTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended3.ToString()) innerNode.InnerText = this._parametersTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended4.ToString()) innerNode.InnerText = this._globalParametersTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended5.ToString()) innerNode.InnerText = this._instrumentControlTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended6.ToString()) innerNode.InnerText = this._barcodeReaderControlTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended7.ToString()) innerNode.InnerText = this._dutControlTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended8.ToString()) innerNode.InnerText = this._displayControlTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended9.ToString()) innerNode.InnerText = this._printerTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended10.ToString()) innerNode.InnerText = this._runTestConditionsTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended11.ToString()) innerNode.InnerText = this._dataCollectionTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended12.ToString()) innerNode.InnerText = this._pathLossViewTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended13.ToString()) innerNode.InnerText = this._fixtureControlTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended14.ToString()) innerNode.InnerText = this._systemIOPortsTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended15.ToString()) innerNode.InnerText = this._testPlanStartTab;
                                        if (innerNode.Name == ExecSettingsFileNodes.Extended16.ToString()) innerNode.InnerText = this._testPlanAttributes;
                                    }
                                }
                            }
                        }
                        if (load)
                        {
                            this.loaded = true;
                        }
                        else
                        {
                            xmlDoc.Save(Path);
                            changesNotSaved = false;
                            fileNeedsRestart = true;
                        }
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if(!this.loaded && !load)
                throw new ExecSettingsCantWriteIfNotLoadedException(this);
            }
            return false;
        }

        /// <summary>
        /// Loads the file values into the variables
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            return LoadWrite(true);
        }

        /// <summary>
        /// Loads the exec settings file
        /// </summary>
        /// <param name="force">Indicates if a force load is needed</param>
        /// <returns></returns>
        public bool Load(bool force)
        {
            if(force) loaded = false;
            return LoadWrite(true);
        }

        /// <summary>
        /// Write the value of the varables into the file
        /// </summary>
        /// <returns></returns>
        public bool Write()
        {
            return LoadWrite(false);
        }

        /// <summary>
        /// Checks all the settings and return if they have their default values
        /// </summary>
        /// <returns></returns>
        public bool ResetAllSettings()
        {
            return ResetAllSettings(true);
        }

        /// <summary>
        /// Checks all the settings and return if they have their default values
        /// </summary>
        /// <returns></returns>
        public bool ResetAllSettings(bool writeFile)
        {
            bool changeMade = ResetMandatorySettings(false);

            if (_resultsFilePath != DEFAULT_RESULTS_FILE_PATH) { _resultsFilePath = DEFAULT_RESULTS_FILE_PATH; changeMade = true; }
            if (_defineTestPlanTab != DEFAULT_DEFINE_TEST_PLAN_TAB) { _defineTestPlanTab = DEFAULT_DEFINE_TEST_PLAN_TAB; changeMade = true; }
            if (_specificationsTab != DEFAULT_SPECIFICATIONS_TAB) { _specificationsTab = DEFAULT_SPECIFICATIONS_TAB; changeMade = true; }
            if (_parametersTab != DEFAULT_PARAMETERS_TAB) { _parametersTab = DEFAULT_PARAMETERS_TAB; changeMade = true; }
            if (_globalParametersTab != DEFAULT_GLOBAL_PARAMETERS_TAB) { _globalParametersTab = DEFAULT_GLOBAL_PARAMETERS_TAB; changeMade = true; }
            if (_instrumentControlTab != DEFAULT_INSTRUMENT_CONTROL_TAB) { _instrumentControlTab = DEFAULT_INSTRUMENT_CONTROL_TAB; changeMade = true; }
            if (_barcodeReaderControlTab != DEFAULT_BARCODE_READER_CONTROL_TAB) { changeMade = true; }
            if (_dutControlTab != DEFAULT_DUT_CONTROL_TAB) { _dutControlTab = DEFAULT_DUT_CONTROL_TAB; changeMade = true; }
            if (_displayControlTab != DEFAULT_DISPLAY_CONTROL_TAB) { _displayControlTab = DEFAULT_DISPLAY_CONTROL_TAB; changeMade = true; }
            if (_printerTab != DEFAULT_PRINTER_TAB) { _printerTab = DEFAULT_PRINTER_TAB; changeMade = true; }
            if (_runTestConditionsTab != DEFAULT_RUN_TEST_CONDITIONS_TAB) { _runTestConditionsTab = DEFAULT_RUN_TEST_CONDITIONS_TAB; changeMade = true; }
            if (_dataCollectionTab != DEFAULT_DATA_COLLECTION_TAB) { _dataCollectionTab = DEFAULT_DATA_COLLECTION_TAB; changeMade = true; }
            if (_pathLossViewTab != DEFAULT_PATHLOSS_VIEW_TAB) { _pathLossViewTab = DEFAULT_PATHLOSS_VIEW_TAB; changeMade = true; }
            if (_fixtureControlTab != DEFAULT_FIXTURE_CONTROL_TAB) { _fixtureControlTab = DEFAULT_FIXTURE_CONTROL_TAB; changeMade = true; }
            if (_systemIOPortsTab != DEFAULT_SYSTEM_IOPORTS_TAB) { _systemIOPortsTab = DEFAULT_SYSTEM_IOPORTS_TAB; changeMade = true; }
            if (_testPlanStartTab != DEFAULT_TESTPLAN_START_TAB) { _testPlanStartTab = DEFAULT_TESTPLAN_START_TAB; changeMade = true; }
            if (_testPlanAttributes != DEFAULT_TESTPLAN_ATTRIBUTES_TAB) { _testPlanAttributes = DEFAULT_TESTPLAN_ATTRIBUTES_TAB; changeMade = true; }

            if (changeMade) changesNotSaved = true;
            if (writeFile && changeMade)
            {
                Write();
            }
            return changeMade;
        }

        /// <summary>
        /// Checks the settings that should never change (doesn't include tabs and results file path)
        /// </summary>
        /// <returns>Return if a change was made to any value</returns>
        public bool ResetMandatorySettings()
        {
            return ResetMandatorySettings(true);
        }

        /// <summary>
        /// Checks the settings that should never change (doesn't include tabs and results file path)
        /// </summary>
        /// <param name="writeFile">Sets if the xml is going to be written inside the method</param>
        /// <returns>Return if a change was made to any value</returns>
        public bool ResetMandatorySettings(bool writeFile)
        {
            bool changeMade = false;
            if (_password != DEFAULT_PASSWORD) { _password = DEFAULT_PASSWORD; changeMade = true; }
            if (_saveResultsToFile != DEFAULT_SAVE_RESULTS_TO_FILE) { _saveResultsToFile = DEFAULT_SAVE_RESULTS_TO_FILE; changeMade = true; }
            if (_resultsFileType != DEFAULT_RESULTS_FILE_TYPE) { _resultsFileType = DEFAULT_RESULTS_FILE_TYPE; changeMade = true; }
            if (_keepResultsFromAborted != DEFAULT_KEEP_RESULTS_FROM_ABORTED) { _keepResultsFromAborted = DEFAULT_KEEP_RESULTS_FROM_ABORTED; changeMade = true; }
            if (_resultsFilePathSecondary != DEFAULT_RESULTS_FILE_PATH_SECONDARY) { _resultsFilePathSecondary = DEFAULT_RESULTS_FILE_PATH_SECONDARY; changeMade = true; }
            if (_displayLastMeas != DEFAULT_DISPLAY_LAST_MEAS) { _displayLastMeas = DEFAULT_DISPLAY_LAST_MEAS; changeMade = true; }
            if (_printType != DEFAULT_PRINT_TYPE) { _printType = DEFAULT_PRINT_TYPE; changeMade = true; }            
            if (changeMade) changesNotSaved = true;
            if (writeFile && changeMade)
            {
                Write();
            }
            return changeMade;
        }

        #endregion

        #region Getters and Setters

        public bool ChangesNotSaved
        {
            get
            {
                return this.changesNotSaved;
            }
        }

        public string ResultsFilePath
        {
            get
            {
                return this._resultsFilePath;
            }
        }

        public string PathLossFile
        {
            get
            {
                return this._pathLossFile;
            }
            set
            {
                if (this._pathLossFile != value) changesNotSaved = true;
                this._pathLossFile = value;                
            }
        }

        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                this._database = value;
            }
        }

        public string DefineTestPlanTab
        {
            get
            {
                return this._defineTestPlanTab;
            }
        }

        public string SpecificationsTab
        {
            get
            {
                return this._specificationsTab;
            }
        }

        public string ParametersTab
        {
            get
            {
                return this._parametersTab;
            }
        }

        public string GlobalParametersTab
        {
            get
            {
                return this._globalParametersTab;
            }
        }

        public string InstrumentControlTab
        {
            get
            {
                return this._instrumentControlTab;
            }
        }

        public string BarcodeReaderControlTab
        {
            get
            {
                return this._barcodeReaderControlTab;
            }
        }

        public string DutControlTab
        {
            get
            {
                return this._dutControlTab;
            }
        }

        public string DisplayControlTab
        {
            get
            {
                return this._displayControlTab;
            }
        }

        public string PrinterTab
        {
            get
            {
                return this._printerTab;
            }
        }

        public string RunTestConditionsTab
        {
            get
            {
                return this._runTestConditionsTab;
            }
        }

        public string DataCollectionTab
        {
            get
            {
                return this._dataCollectionTab;
            }
        }

        public string PathLossViewTab
        {
            get
            {
                return this._pathLossViewTab;
            }
        }

        public string FixtureControlTab
        {
            get
            {
                return this._fixtureControlTab;
            }
        }

        public string SystemIOPortsTab
        {
            get
            {
                return this._systemIOPortsTab;
            }
        }

        public string TestPlanStartTab
        {
            get
            {
                return this._testPlanStartTab;
            }
        }

        public string TestPlanAttributes
        {
            get
            {
                return this._testPlanAttributes;
            }
        }

        #endregion

    }
}
