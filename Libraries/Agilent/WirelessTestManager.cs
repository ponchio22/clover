using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using Valutech.Agilent.Exceptions;
using Valutech.IO;
using System.Timers;
using Valutech.FileSync;

namespace Valutech.Agilent
{

    public delegate void WTMRestartedEventHandler(WTMVersion version);
    
    public delegate void WTMTestPlanCheckForUpdatesErrorEventHandler(WTMTestPlanCheckForUpdatesErrorType error,Exception description);

    public delegate void WTMTestPlanUpdateErrorEventHandler(WTMTestPlanUpdateErrorType error, Exception description);

    public delegate void WTMTestPlanUpdatedEventHandler(OEM oem);

    public delegate void WTMTestPlanUpdatedSuccesfullHandler(OEM oem);

    public delegate void WTMTestPlanUpdateCheckSuccesfullHandler(OEM oem);

    public delegate void ModelSelectedHandler(Model model);

    public delegate void ModelInUseChangedEventHandler(Model model);

    public delegate void WTMInformationRetrieveEventHandler(object sender, EventArgs args);

    public delegate void WTMTestplanChangedEventHandler(string testplan);
    
    public enum WTMTestPlanCheckForUpdatesErrorType
    {
        UNAUTHORIZED_ACCESS,
        FILE_NOT_FOUND,
        DIRECTORY_NOT_FOUND,
        IO_EXCEPTION,
        UNKNOWN_ERROR
    }

    public enum WTMTestPlanUpdateErrorType
    {
        UNAUTHORIZED_ACCESS,
        FILE_NOT_FOUND,
        DIRECTORY_NOT_FOUND,
        IO_EXCEPTION,
        UNKNOWN_ERROR
    }

    /// <summary>
    /// Singleton class for the wireless test manager program
    /// </summary>
    public sealed class WirelessTestManager
    {

        public VERSION version = VERSION.NONE;
        
        public STATE state = STATE.CLOSED;
        
        public WTM_ACTIVITY activity = WTM_ACTIVITY.FIRST_TIME_START;
        
        private string testplan = string.Empty;
        
        private Model model = null;
        
        private bool gettingStatus = true;
        
        private bool abortRequested = false;

        //private System.Timers.Timer UpdaterThread = new System.Timers.Timer(100);
        private Thread UpdaterThread;
                
        #region Constants Declaration

        /// <summary>
        /// Registry path
        /// </summary>
        public const string WTM_REGISTRY_PATH = "SOFTWARE\\Agilent\\Wireless\\Wireless Test Manager Framework Net";

        /// <summary>
        /// Versions Registry path
        /// </summary>
        public const string WTM_VERSIONS_REGISTRY_PATH = "SOFTWARE\\Agilent\\Wireless\\Wireless Test Manager Selectable Applications";

        /// <summary>
        /// Registry wtm path variable name
        /// </summary>
        public const string WTM_REGISTRY_PATH_PATH_NAME = "Path";

        /// <summary>
        /// Path not found
        /// </summary>
        public const string PATH_NOT_FOUND = "not_found";

        /// <summary>
        /// Version process not found
        /// </summary>
        public const string PROCESS_NOT_FOUND = "process_not_found";

        /// <summary>
        /// Directorio donde se
        /// </summary>
        public const string ONLINE_PLANS_DIRECTORY = @"\\file-server\files\58_RF\PlansDirectory.xml";

        #endregion

        #region Timer

        private System.Timers.Timer TestPlanUpdaterTimer = new System.Timers.Timer(5000);

        #endregion

        #region Object Declaration

        /// <summary>
        /// Private instance for singleton use
        /// </summary>
        private static WirelessTestManager instance;
        
        public event WTMRestartedEventHandler WTMRestarted;

        public event WTMRestartedEventHandler WTMClosed;

        public event WTMTestPlanCheckForUpdatesErrorEventHandler WTMTestPlanCheckForUpdatesError;

        public event WTMTestPlanUpdateErrorEventHandler WTMTestPlanUpdateError;

        public event WTMTestPlanUpdateCheckSuccesfullHandler WTMTestPlanUpdateCheckSuccessfull;

        public event WTMTestPlanUpdatedSuccesfullHandler WTMTestPlanUpdatedSuccessfull;

        public event ModelSelectedHandler ModelSelected;

        public event ModelInUseChangedEventHandler ModelInUseChanged;

        #region WTM Information Events

        public event WTMInformationRetrieveEventHandler StateChanged;
        public event WTMInformationRetrieveEventHandler VersionChanged;
        public event WTMInformationRetrieveEventHandler ActivityChanged;
        public event WTMTestplanChangedEventHandler TestplanChanged;

        #endregion

        private ArrayList versions = new ArrayList();

        #endregion

        #region Singleton

        /// <summary>
        /// Private Contructor, works as singleton
        /// </summary>
        private WirelessTestManager()
        {
            instance = this;
            if (this.IsRunning() != null) this.model = GetModelInUse();
            TestPlanUpdaterTimer.Elapsed += new ElapsedEventHandler(TestPlanUpdaterTimer_Elapsed);           
            TestPlanUpdaterTimer.Enabled = true;
            TestPlanUpdaterTimer.Start();
            /*
            UpdaterThread.Elapsed += new System.Timers.ElapsedEventHandler(UpdaterThread_Elapsed);
            UpdaterThread.Enabled = true;
            UpdaterThread.Start();*/
            UpdaterThread = new Thread(Update);
            UpdaterThread.Start();
        }

        /// <summary>
        /// Gets the current instance or creates a new one if no one is there
        /// </summary>
        /// <returns></returns>
        public static WirelessTestManager GetInstance()
        {            
            if (instance == null) instance = new WirelessTestManager();
            return instance;
        }

        #endregion

        void UpdaterThread_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Update();
        }


        void TestPlanUpdaterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PeriodicallyCheckForTestPlanUpdates();
        }

        #region General  Wireless Test Manager Information

        /// <summary>
        /// Gets the path to the wireless test manager from the registry
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(WTM_REGISTRY_PATH, false);
            if (regKey != null)
            {
                return regKey.GetValue(WTM_REGISTRY_PATH_PATH_NAME, PATH_NOT_FOUND).ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region Threads

        /// <summary>
        /// Stop the Wireless Test Manager thread
        /// </summary>
        public void Stop()
        {
            UpdaterThread.Abort();
            TestPlanUpdaterTimer.Stop();
            WirelessTestManagerStatus.GetInstance().Stop();
        }

        #endregion

        #region WTM State Information/Actions

        /// <summary>
        /// Closes the currently running version of the wtm
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            WTMVersion version = IsRunning();
            if (version != null)
            {
                bool result = version.Close();
                if (result && WTMClosed != null) WTMClosed(version);
                return result;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Restarts currently running version of the wtm
        /// </summary>
        public bool Restart()
        {
            WTMVersion version = IsRunning();
            if (version != null)
            {
                if (version.Close())
                {
                    if (version.Open())
                    {
                        if (WTMRestarted != null) WTMRestarted(version);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the currently running version if any
        /// </summary>
        /// <returns></returns>
        public WTMVersion IsRunning()
        {            
            ArrayList versions = GetInstalledVersions();
            foreach (WTMVersion version in versions)
            {
                if (version.IsRunning())
                {
                    return version;
                }
            }
            return null;
        }

        /// <summary>
        /// Indicates if the wtm is open
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return (state == STATE.OPEN);
        }

        public WTMVersion GetRunningVersion()
        {
            ArrayList versions = GetInstalledVersions();
            foreach (WTMVersion version in versions)
            {
                if (version.IsRunning())
                {
                    return version;
                }
            }
            return null;
        }

        #endregion
        
        #region TestPlan Updating

        /// <summary>
        /// Checks every 5 minutes for any test plan updates
        /// </summary>
        public void PeriodicallyCheckForTestPlanUpdates()
        {
            TestPlanUpdaterTimer.Enabled = false;
            try
            {
                WTMVersion runningVersion = IsRunning();
                if (runningVersion != null)
                {
                    OEM oemInUse = runningVersion.GetOEMInUse();
                    if (oemInUse != null && oemInUse.TestPlan != String.Empty)
                    {
                        CheckForTestPlanUpdates(oemInUse);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Periodically check for test plan updates exceptions: " + ex.ToString());
            }
            TestPlanUpdaterTimer.Enabled = true;
        }

        /// <summary>
        /// Checks for updates on the test plan
        /// </summary>
        /// <param name="oem"></param>
        public void CheckForTestPlanUpdates(OEM oem)
        {
            try
            {
                Process thisProc = Process.GetCurrentProcess();
                if (thisProc.ProcessName != "devenv")
                {
                    string localTestPlanLocation = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName(ONLINE_PLANS_DIRECTORY));                    
                    
                    if (!File.Exists(localTestPlanLocation))
                    {
                        File.Copy(ONLINE_PLANS_DIRECTORY, localTestPlanLocation, true);
                    }
                    if (!FileVersionHandler.CompareVersions(localTestPlanLocation, ONLINE_PLANS_DIRECTORY))
                    {
                        File.Copy(ONLINE_PLANS_DIRECTORY, localTestPlanLocation, true);
                    }
                    
                    PlansDirectory plansDirectory = PlansDirectory.GetInstance();
                    plansDirectory.UpdateDataFromFile(localTestPlanLocation);
                    ArrayList plans = plansDirectory.Plans;
                    foreach (Plan plan in plans)
                    {
                        if (plan.FileName == oem.TestPlan)
                        {
                            if (plan.Version != oem.TestPlanVersion)
                            {
                                UpdateTestPlan(oem, plan.Version);
                                if (WTMTestPlanUpdateCheckSuccessfull != null) WTMTestPlanUpdateCheckSuccessfull(oem);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                if (WTMTestPlanCheckForUpdatesError != null) WTMTestPlanCheckForUpdatesError(WTMTestPlanCheckForUpdatesErrorType.FILE_NOT_FOUND,ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (WTMTestPlanCheckForUpdatesError != null) WTMTestPlanCheckForUpdatesError(WTMTestPlanCheckForUpdatesErrorType.UNAUTHORIZED_ACCESS, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                if (WTMTestPlanCheckForUpdatesError != null) WTMTestPlanCheckForUpdatesError(WTMTestPlanCheckForUpdatesErrorType.DIRECTORY_NOT_FOUND, ex);
            }
            catch (IOException ex)
            {
                if (WTMTestPlanCheckForUpdatesError != null) WTMTestPlanCheckForUpdatesError(WTMTestPlanCheckForUpdatesErrorType.IO_EXCEPTION, ex);
            }
            catch(Exception ex)
            {
                if (WTMTestPlanCheckForUpdatesError != null) WTMTestPlanCheckForUpdatesError(WTMTestPlanCheckForUpdatesErrorType.UNKNOWN_ERROR, ex);
            }
        }

        /// <summary>
        /// Updates the testplan
        /// </summary>
        /// <param name="oem"></param>
        public void UpdateTestPlan(OEM oem, string version)
        {
            WTMVersion runningVersion = this.IsRunning();
            try
            {                
                string remoteTargetTestPlan = Path.Combine(Path.GetDirectoryName(ONLINE_PLANS_DIRECTORY), Path.GetFileName(oem.TestPlan));
                string localTargetTestPlan = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName(oem.TestPlan));
                ExecSetting execSetting = oem.ExecSetting;
                execSetting.Load();
                string database = (execSetting.Database.Length > 3)? Path.Combine(oem.ModelsPath, Path.GetFileName(execSetting.Database)):Path.Combine(oem.ModelsPath, Path.GetFileName(remoteTargetTestPlan));                
                File.Copy(remoteTargetTestPlan, localTargetTestPlan, true);                
                if (runningVersion != null) this.Close();
                Thread.Sleep(2000);
                File.Copy(localTargetTestPlan, database, true);
                execSetting.Database = database;
                execSetting.Write();
                oem.TestPlanVersion = version;
                if (WTMTestPlanUpdatedSuccessfull != null) WTMTestPlanUpdatedSuccessfull(oem);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (WTMTestPlanUpdateError != null) WTMTestPlanUpdateError(WTMTestPlanUpdateErrorType.UNAUTHORIZED_ACCESS, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                if (WTMTestPlanUpdateError != null) WTMTestPlanUpdateError(WTMTestPlanUpdateErrorType.DIRECTORY_NOT_FOUND, ex);
            }
            catch (FileNotFoundException ex)
            {
                if (WTMTestPlanUpdateError != null) WTMTestPlanUpdateError(WTMTestPlanUpdateErrorType.FILE_NOT_FOUND, ex);
            }
            catch (IOException ex)
            {
                if (WTMTestPlanUpdateError != null) WTMTestPlanUpdateError(WTMTestPlanUpdateErrorType.IO_EXCEPTION, ex);
            }
            catch(Exception ex)
            {
                if (WTMTestPlanUpdateError != null) WTMTestPlanUpdateError(WTMTestPlanUpdateErrorType.UNKNOWN_ERROR, ex);
            }
            if (runningVersion != null && IsRunning() == null) runningVersion.Open();
        }

        #endregion

        #region Version Managing

        /// <summary>
        /// Gets the installed versions according to the registry entries
        /// </summary>
        /// <returns></returns>
        public ArrayList LoadInstalledVersions()
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(WTM_VERSIONS_REGISTRY_PATH, false);
            ArrayList versions = new ArrayList();
            if (regKey != null)
            {
                string[] keys = regKey.GetSubKeyNames();
                foreach (string name in keys)
                {
                    WTMVersion version = new WTMVersion(name);
                    versions.Add(version);
                }
            }
            return versions;
        }

        /// <summary>
        /// Get the installed versions
        /// </summary>
        /// <returns></returns>
        public ArrayList GetInstalledVersions()
        {
            if (versions.Count == 0) versions = LoadInstalledVersions();
            return versions;
        }

        /// <summary>
        /// Get the version object from the name
        /// </summary>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public WTMVersion GetVersion(string versionName)
        {
            ArrayList versions = GetInstalledVersions();
            foreach (WTMVersion version in versions)
            {
                if (version.Name == versionName) return version;
            }
            return null;
        }

        #endregion

        #region Models Managing

        /// <summary>
        /// Validates a model file to see if its really a path loss file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ValidateModel(Model model)
        {
            if (System.IO.Path.GetExtension(model.Path).ToLower() == Model.Extension.ToLower())
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    XmlNodeList list;
                    xml.Load(model.Path);
                    list = xml.GetElementsByTagName(Model.PATH_EFFECT_TABLE);
                    if (list != null)
                    {
                        if (list.Count > 0) return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Gets the current model in use
        /// </summary>
        /// <returns></returns>
        public Model GetModelInUse()
        {
            Model model = null;
            try
            {
                WTMVersion version = this.IsRunning();
                if (version != null)
                {
                    OEM oem = version.GetOEMInUse();
                    oem.ExecSetting.Load(true);
                    model = oem.GetModelFromPath(oem.ExecSetting.PathLossFile);
                    
                }
            }
            catch
            {
            }
            return model;
        }

        /// <summary>
        /// Makes the directory rename if needed and the changes in the exec settings file if needed
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="Valutech.Agilent.UnableToCloseWTMException">Thrown when the wtm was not able to get closed by the system</exception>
        /// <exception cref="Valutech.Agilent.UnableToArchiveOEMException">Thrown when the oem was not able to get archived</exception>
        /// <exception cref="Valutech.Agilent.UnableToUseOEMException">Thrown when the oem was not able to get used</exception>
        /// <exception cref="Valutech.Agilent.UnableToCreateExecSettingBackupException">Thrown when the ExecSetting backup file could not be created</exception>
        public void UseModel(Model model)
        {
            //Use OEM, a wtm close can happen
            OEM inUseOEM = model.OEM.Version.GetOEMInUse();
            if (inUseOEM != null && inUseOEM.Name != model.OEM.Name) inUseOEM.Archive();
            model.OEM.Use();
            if (ModelSelected != null) ModelSelected(model);

            //Check for the exec settings, the reload just happens if the wtm has been closed
            ExecSetting execSetting = model.OEM.ExecSetting;
            execSetting.Load();
            execSetting.ResetAllSettings();
            execSetting.PathLossFile = model.InUsePath;

            if (execSetting.ChangesNotSaved) execSetting.Write();

            WTMVersion runningVersion = IsRunning();
            if (execSetting.NeedsRestart && runningVersion != null)
            {
                Restart();
            }
            else
            {
                model.OEM.Version.Open();
            }
            
        }

        #endregion

        

        #region WTM Information Enums

        public enum STATE
        {
            OPENING,
            OPEN,
            CLOSED,
            NONE
        }

        public enum VERSION
        {
            AGILENT_E6560,
            AGILENT_E6567,
            AGILENT_E6568,
            POST_E8285,
            NONE
        }

        public enum WTM_ACTIVITY
        {
            FIRST_TIME_START,
            TEST_FINISHED,
            PERFORMING_CDMA_PAGE,
            WAITING_TO_ANSWER_CALL,
            CHECKING_VOICE_QUALITY,
            TESTING,
            ABORTING,
            NONE
        }

        #endregion

        #region Pointers

        private IntPtr mainHWnd = IntPtr.Zero;
        private IntPtr wtmHwnd = IntPtr.Zero;
        private IntPtr stopBtnHwnd = IntPtr.Zero;
        private IntPtr runBtnHwnd = IntPtr.Zero;
        private IntPtr viewTestCondBtnHwnd = IntPtr.Zero;
        private IntPtr dutSerialNumberHwnd = IntPtr.Zero;
        private IntPtr dutSerialNumberLblHwnd = IntPtr.Zero;
        private IntPtr operatorNumberHwnd = IntPtr.Zero;
        private IntPtr operatorNumberLblHwnd = IntPtr.Zero;

        #endregion

        void Update()
        {
            while (true)
            {
                LoadVersionAndState();
                CloseAbortQuestionWindow();
                CloseWrongDatabaseWindow();
                ResizeWTM();
                if (abortRequested) SendAbortTestCommand();                
                Thread.Sleep(50);
            }
        }

        private void ResizeWTM()
        {
            LoadVersionAndState();
            if (this.activity == WTM_ACTIVITY.TESTING)
            {/*
                if (wtmHwnd != IntPtr.Zero)
                {
                    AppManipulation.MoveWindow(wtmHwnd, 0, 0, (Screen.PrimaryScreen.WorkingArea.Width - 300), Screen.PrimaryScreen.WorkingArea.Height, true);
                }
                IntPtr rfControl = AppManipulation.FindWindowByWindowName("RF Control Plus");
                if (rfControl != IntPtr.Zero)
                {
                    AppManipulation.MoveWindow(rfControl, (Screen.PrimaryScreen.WorkingArea.Width - 300), 0, 350, Screen.PrimaryScreen.WorkingArea.Height - 320, true);
                }*/
            }
        }

        private void CloseWrongDatabaseWindow()
        {
            IntPtr errorWindow = IntPtr.Zero;
            IntPtr errorWindowYeshWnd = IntPtr.Zero;
            if (version != VERSION.POST_E8285)
            {
                errorWindow = AppManipulation.FindWindowByWindowName("Incorrect Database Found");
                if (errorWindow != IntPtr.Zero) errorWindowYeshWnd = AppManipulation.FindChildWindowByWindowName(errorWindow, "&Yes");
                if (errorWindowYeshWnd != IntPtr.Zero)
                {
                    AppManipulation.SendClickToWindow(errorWindowYeshWnd);                    
                }
            }
        }

        private void LoadVersionAndState()
        {

            STATE currentState = state;
            VERSION currentVersion = version;
            WTM_ACTIVITY currentActivity = activity;
            string currentTestplan = testplan;

            //Check if Wtm is open
            mainHWnd = AppManipulation.FindWindowByClassName("WindowsForms10.Window.8.app3");

            //Set version
            if (mainHWnd != IntPtr.Zero)
            {
                string wName = AppManipulation.GetWindowName(mainHWnd);
                wtmHwnd = mainHWnd;                
                if (wName.IndexOf("E6560") > -1)
                {
                    version = VERSION.AGILENT_E6560;
                }
                else if (wName.IndexOf("E6567") > -1)
                {
                    version = VERSION.AGILENT_E6567;
                }
                else if (wName.IndexOf("E6568") > -1)
                {
                    version = VERSION.AGILENT_E6568;
                }

                //Get the testplan directly from window
                bool next = false;
                foreach (IntPtr hWnd in AppManipulation.GetChildWindows(mainHWnd))
                {                    
                    if (next == true)
                    {
                        if (currentState == STATE.OPEN)
                        {
                            this.testplan = AppManipulation.GetText(hWnd);                         
                        }
                        next = false;
                    }
                    if (AppManipulation.GetText(hWnd).IndexOf("DUT serial number")>-1)
                    {
                        next = true;
                    }
                }       

            }
            else
            {
                wtmHwnd = AppManipulation.FindWindowByWindowName("PoST- 8924C/E & E8285A CDMA Mobile Tests");
                mainHWnd = wtmHwnd;
                if (wtmHwnd != IntPtr.Zero) version = VERSION.POST_E8285;
            }

            //Set state
            if (mainHWnd != IntPtr.Zero)
            {
                if (wtmHwnd != IntPtr.Zero)
                {
                    state = STATE.OPEN;
                }
                else
                {
                    state = STATE.OPENING;
                }
            }
            else
            {
                state = STATE.CLOSED;
            }

            //Set current wtm activity            
            if (mainHWnd != IntPtr.Zero && state == STATE.OPEN && version != VERSION.NONE)
            {
                if (version != VERSION.POST_E8285)
                {
                    try
                    {
                        #region Set the activity for the Wtm versions
                        if (this.gettingStatus)
                        {
                            if (!AppManipulation.IsWindowMinimized(wtmHwnd))
                            {
                                //Get all the handlers for all the Wtm versions
                                int ContainerCount = AppManipulation.FindChildWindowCountByClassName(mainHWnd, "WindowsForms10.Window.8.app3");
                                if (ContainerCount > 0)
                                {
                                    ContainerCount = (ContainerCount > 30) ? 30 : ContainerCount;
                                    activity = WTM_ACTIVITY.NONE;
                                    string[] captions;
                                    bool somethingFound = false;
                                    for (int j = ContainerCount - 3; j < ContainerCount; j++)
                                    {
                                        IntPtr statusContainer = AppManipulation.FindChildWindowByClassNameByIndex(mainHWnd, "WindowsForms10.Window.8.app3", j);//27                            
                                        if (statusContainer != IntPtr.Zero)
                                        {
                                            int count = AppManipulation.FindChildWindowCountByClassName(statusContainer, "WindowsForms10.msctls_statusbar32.app3");//6 o 7                                   

                                            for (int i = 6; i < count; i++)
                                            {
                                                IntPtr statusPtr = AppManipulation.FindChildWindowByClassNameByIndex(statusContainer, "WindowsForms10.msctls_statusbar32.app3", i);
                                                if (statusPtr != IntPtr.Zero)
                                                {

                                                    Valutech.IO.StatusBar status = new Valutech.IO.StatusBar(statusPtr);
                                                    captions = status.Captions;
                                                    if (captions.Length > 1)
                                                    {
                                                        string statusText = captions[1];
                                                        if (statusText != string.Empty)
                                                        {
                                                            somethingFound = true;
                                                            if (activity == WTM_ACTIVITY.NONE)
                                                            {
                                                                if (statusText == "	") activity = WTM_ACTIVITY.FIRST_TIME_START;
                                                                Regex regex = new Regex("Performing CDMAVoiceQuality . . .|Performing GSMMobileInitiatedCall . . .");
                                                                if (regex.IsMatch(statusText)) activity = WTM_ACTIVITY.CHECKING_VOICE_QUALITY;
                                                                regex = new Regex("Performing CDMAPage . . .");
                                                                if (regex.IsMatch(statusText)) activity = WTM_ACTIVITY.PERFORMING_CDMA_PAGE;
                                                                regex = new Regex("Performing WCDMAOrigination . . .");
                                                                if (regex.IsMatch(statusText)) activity = WTM_ACTIVITY.PERFORMING_CDMA_PAGE;
                                                                regex = new Regex("Passed = [0-9]{1,}[ ]{1,}Failed = [0-9]{1,}|Test Plan Aborted");
                                                                if (regex.IsMatch(statusText)) activity = WTM_ACTIVITY.TEST_FINISHED;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if ((somethingFound && activity == WTM_ACTIVITY.NONE))
                                    {
                                        activity = WTM_ACTIVITY.TESTING;
                                    }
                                }                         
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not getting status");
                        }
                        #endregion
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        #region Set the activity for the 8285 equipment

                        IntPtr statusContainer = AppManipulation.FindWindowByWindowName("PoST Operator Instructions");
                        if (statusContainer != IntPtr.Zero)
                        {
                            IntPtr statusPtr = AppManipulation.FindChildWindowByClassName(statusContainer, "RichTextWndClass");
                            if (statusPtr != IntPtr.Zero)
                            {
                                string caption = AppManipulation.GetText(statusPtr);
                                if (caption != String.Empty)
                                {
                                    activity = WTM_ACTIVITY.PERFORMING_CDMA_PAGE;
                                    Regex regex = new Regex("Waiting for operator to answer call");
                                    if (regex.IsMatch(caption)) activity = WTM_ACTIVITY.WAITING_TO_ANSWER_CALL;
                                    regex = new Regex("Speak into the phone");
                                    if (regex.IsMatch(caption)) activity = WTM_ACTIVITY.CHECKING_VOICE_QUALITY;
                                }
                            }
                        }
                        else
                        {
                            if (currentActivity != WTM_ACTIVITY.WAITING_TO_ANSWER_CALL)
                            {
                                GetBtnHwnds();
                                if (stopBtnHwnd != IntPtr.Zero)
                                {
                                    if (AppManipulation.IsWindowVisible(stopBtnHwnd))
                                    {
                                        activity = WTM_ACTIVITY.TESTING;
                                    }
                                    else
                                    {
                                        activity = WTM_ACTIVITY.TEST_FINISHED;
                                    }
                                }
                            }
                            else
                            {
                                activity = currentActivity;
                            }
                        }

                        #endregion

                    }
                    catch { }
                }
            }
            else
            {
                stopBtnHwnd = IntPtr.Zero;
                runBtnHwnd = IntPtr.Zero;
                dutSerialNumberHwnd = IntPtr.Zero;
            }

            //Send events depending on the change
            if (currentState != state)
            {
                if (StateChanged != null) StateChanged(this, EventArgs.Empty);
                if (state == STATE.OPEN)
                {
                    this.model = GetModelInUse();
                    if (ModelInUseChanged != null) ModelInUseChanged(this.model);
                }
            }
            if (currentActivity != activity)
            {
                if (ActivityChanged != null) ActivityChanged(this, EventArgs.Empty);
            }
            if (currentVersion != version)
            {
                if (VersionChanged != null) VersionChanged(this, EventArgs.Empty);
            }            
            if (currentTestplan != this.testplan && currentState == STATE.OPEN)
            {                
                if (TestplanChanged != null) TestplanChanged(this.testplan);
            }
        }

        private void GetBtnHwnds()
        {            
            if (version != VERSION.NONE)
            {                
                if (version != VERSION.POST_E8285)
                {                    
                    IntPtr stopContainer;
                    runBtnHwnd = IntPtr.Zero;
                    stopContainer = AppManipulation.FindChildWindowByClassNameByIndex(wtmHwnd, "WindowsForms10.Window.8.app3", 10);
                    stopBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "Stop Test Plan");
                    viewTestCondBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "View Test Conditions");
                    runBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "Run Test Plan");
                    dutSerialNumberHwnd = AppManipulation.FindChildWindowByClassNameByIndex(stopContainer, "WindowsForms10.EDIT.app3", 3);
                    dutSerialNumberLblHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "DUT serial number:");
                    operatorNumberHwnd = AppManipulation.FindChildWindowByClassNameByIndex(stopContainer, "WindowsForms10.EDIT.app3", 1);
                    operatorNumberLblHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "Operator:");
                }
                else
                {                    
                    IntPtr mdi = IntPtr.Zero;
                    IntPtr pew = IntPtr.Zero;
                    IntPtr tabs = IntPtr.Zero;
                    IntPtr cont = IntPtr.Zero;
                    mdi = AppManipulation.FindChildWindowByClassName(wtmHwnd, "MDI Client");
                    if (mdi != IntPtr.Zero) pew = AppManipulation.FindChildWindowByWindowName(mdi, "PoST Executive Window");
                    if (pew != IntPtr.Zero) tabs = AppManipulation.FindChildWindowByClassNameByIndex(pew, "SSTabCtlWndClass", 3);
                    if (tabs != IntPtr.Zero) cont = AppManipulation.FindChildWindowByWindowName(tabs, "Test Control");
                    runBtnHwnd = AppManipulation.FindChildWindowByWindowName(cont, "&Start Test");
                    stopBtnHwnd = AppManipulation.FindChildWindowByWindowName(cont, "&Stop Testing");                    
                }
            }
        }

        public bool Enabled
        {
            set
            {
                GetBtnHwnds();
                if (version != VERSION.NONE)
                {                    
                    AppManipulation.SetEnabled(runBtnHwnd, value);
                    AppManipulation.SetEnabled(dutSerialNumberHwnd, false);
                    AppManipulation.ShowWindow(dutSerialNumberHwnd, 0);
                    AppManipulation.SetEnabled(operatorNumberHwnd, false);
                    AppManipulation.ShowWindow(operatorNumberHwnd, 0);
                    AppManipulation.ShowWindow(dutSerialNumberLblHwnd, 0);
                    AppManipulation.ShowWindow(operatorNumberLblHwnd, 0);
                }
            }
        }

        public void RunTest()
        {
            abortRequested = false;
            GetBtnHwnds();
            if (version != VERSION.NONE)
            {
                this.activity = WTM_ACTIVITY.TEST_FINISHED;
                Thread.Sleep(1000);                
                AppManipulation.FocusOnWindow(wtmHwnd);
                AppManipulation.SendClickToWindow(runBtnHwnd);
            }
        }

        /// <summary>
        /// Testplan Getter
        /// </summary>
        public string TestPlan
        {
            get
            {
                return this.testplan;
            }
        }

        public Model ModelInUse
        {
            get
            {
                return this.model;
            }
        }

        public void AbortTest()
        {
            abortRequested = true;
            SendAbortTestCommand();
        }

        /// <summary>
        /// Auto answer the abort window in all the versions
        /// </summary>
        public void CloseAbortQuestionWindow()
        {
            IntPtr abortWindow = IntPtr.Zero;
            IntPtr abortYeshWnd = IntPtr.Zero;
            if (version != VERSION.POST_E8285)
            {
                abortWindow = AppManipulation.FindWindowByWindowName("Abort Testing for System 1");
                if (abortWindow != IntPtr.Zero) abortYeshWnd = AppManipulation.FindChildWindowByWindowName(abortWindow, "&Yes");
            }
            else
            {
                abortWindow = AppManipulation.FindWindowByWindowName("Stop the Test");
                if (abortWindow == IntPtr.Zero) abortWindow = AppManipulation.FindWindowByWindowName("Exit PoST");
                if (abortWindow != IntPtr.Zero) abortYeshWnd = AppManipulation.FindChildWindowByWindowName(abortWindow, "&Yes");
            }
            if (abortYeshWnd != IntPtr.Zero)
            {
                AppManipulation.SendClickToWindow(abortYeshWnd);
                abortRequested = false;
            }
        }

        private void SendAbortTestCommand()
        {
            if (version != VERSION.NONE)
            {
                GetBtnHwnds();
                Console.WriteLine("WirelessTestManager.cs - Abort test command sent");
                AppManipulation.SendClickToWindow(stopBtnHwnd);
            }
        }
    }
}
