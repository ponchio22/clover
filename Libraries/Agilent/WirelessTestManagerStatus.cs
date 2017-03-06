using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Valutech.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Valutech.Agilent
{

    public class WirelessTestManagerStatus
    {

        private static WirelessTestManagerStatus instance;

        #region WTM Information Events

        public event WTMInformationRetrieveEventHandler StateChanged;
        public event WTMInformationRetrieveEventHandler VersionChanged;
        public event WTMInformationRetrieveEventHandler ActivityChanged;
        public event WTMTestplanChangedEventHandler TestplanChanged;

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

        #endregion

        #region Variables

        public VERSION version = VERSION.NONE;
        public STATE state = STATE.CLOSED;
        public WTM_ACTIVITY activity = WTM_ACTIVITY.NONE;
        private string testplan = string.Empty;
        private bool gettingStatus = true;
        private System.Timers.Timer UpdaterThread = new System.Timers.Timer(50);

        #endregion

        /// <summary>
        /// Private Contructor, works as singleton
        /// </summary>
        private WirelessTestManagerStatus()
        {
            UpdaterThread.Elapsed += new System.Timers.ElapsedEventHandler(UpdaterThread_Elapsed);
            UpdaterThread.Enabled = true;
            UpdaterThread.Start();
        }

        void UpdaterThread_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Gets the current instance or creates a new one if no one is there
        /// </summary>
        /// <returns></returns>
        public static WirelessTestManagerStatus GetInstance()
        {
            if (instance == null) instance = new WirelessTestManagerStatus();
            return instance;
        }

        /// <summary>
        /// Updates the information of the wireless test manager
        /// </summary>
        public void Update()
        {
            LoadVersionAndState();
        }

        public void Stop()
        {
            UpdaterThread.Stop();
        }

        private void LoadVersionAndState()
        {
            //NOT USED ANYMORE
            STATE currentState = state;
            VERSION currentVersion = version;
            WTM_ACTIVITY currentActivity = activity;
            string currentTestplan = string.Empty;

            //Check if Wtm is open
            mainHWnd = AppManipulation.FindWindowByClassName("WindowsForms10.Window.8.app3");

            //Set version
            if (mainHWnd != IntPtr.Zero)
            {                
                wtmHwnd = AppManipulation.FindWindowByWindowName("E6567C Agilent cdma2000/IS-95/AMPS/1xEV-DO Wireless Test Manager");
                if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6567;
                if (wtmHwnd == IntPtr.Zero)
                {
                    wtmHwnd = AppManipulation.FindWindowByWindowName("Agilent E6567D cdma2000/IS-95/AMPS/1xEV-DO/LTE Wireless Test Manager");
                    if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6567;
                }
                if (wtmHwnd == IntPtr.Zero)
                {
                    wtmHwnd = AppManipulation.FindWindowByWindowName("Agilent E6567E cdma2000/IS-95/AMPS/1xEV-DO/LTE Wireless Test Manager");                    
                    if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6567;
                }
                if (wtmHwnd == IntPtr.Zero)
                {
                    wtmHwnd = AppManipulation.FindWindowByWindowName("Agilent E6560C cdma2000/IS-95/AMPS Wireless Test Manager");
                    if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6560;
                }
                if (wtmHwnd == IntPtr.Zero)
                {
                    wtmHwnd = AppManipulation.FindWindowByWindowName("Agilent E6568C WCDMA/GSM/GPRS/EGPRS Wireless Test Manager");
                    if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6568;
                }
                if (wtmHwnd == IntPtr.Zero)
                {
                    wtmHwnd = AppManipulation.FindWindowByWindowName("Agilent E6568E WCDMA/GSM/GPRS/EGPRS/LTE Wireless Test Manager");
                    if (wtmHwnd != IntPtr.Zero) version = VERSION.AGILENT_E6568;
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
                                    if (somethingFound && activity == WTM_ACTIVITY.NONE)
                                    {
                                        activity = WTM_ACTIVITY.TESTING;
                                    }
                                }

                                //Get the testplan directly from window
                                int comboboxCount = AppManipulation.FindChildWindowCountByClassName(mainHWnd, "WindowsForms10.COMBOBOX.app3");
                                if (comboboxCount > 0)
                                {
                                    IntPtr comboBoxItem = IntPtr.Zero;
                                    int windowsNumber = 15;
                                    while(comboBoxItem  == IntPtr.Zero && windowsNumber<18)
                                    {
                                        comboBoxItem = AppManipulation.FindChildWindowByClassNameByIndex(mainHWnd, "WindowsForms10.COMBOBOX.app3", windowsNumber);
                                        currentTestplan = AppManipulation.GetText(comboBoxItem);
                                        windowsNumber++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //AppManipulation.WriteInNotepad("Not getting status");
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

                    } catch { }
                }
            }
            else
            {
                stopBtnHwnd = IntPtr.Zero;
                runBtnHwnd = IntPtr.Zero;
            }

            //Send events depending on the change
            if (currentState != state)
            {
                if (StateChanged != null) StateChanged(this, EventArgs.Empty);
            }
            if (currentActivity != activity)
            {
                if (ActivityChanged != null) ActivityChanged(this, EventArgs.Empty);
            }
            if (currentVersion != version)
            {
                if (VersionChanged != null) VersionChanged(this, EventArgs.Empty);
            }
            if (currentTestplan != testplan && currentState == STATE.OPEN && !currentTestplan.Contains("Data terminal ready:") && currentTestplan != string.Empty)
            {
                testplan = currentTestplan;
                if (TestplanChanged != null) TestplanChanged(testplan);
            }

        }

        private void GetBtnHwnds()
        {
            if (version != VERSION.NONE)
            {
                if (version != VERSION.POST_E8285)
                {
                    IntPtr stopContainer = AppManipulation.FindChildWindowByClassNameByIndex(wtmHwnd, "WindowsForms10.Window.8.app3", 10);
                    stopBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "Stop Test Plan");
                    viewTestCondBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "View Test Conditions");
                    runBtnHwnd = AppManipulation.FindChildWindowByWindowName(stopContainer, "Run Test Plan");
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

        public void RunTest()
        {
            GetBtnHwnds();
            if (version != VERSION.NONE)
            {
                AppManipulation.FocusOnWindow(mainHWnd);
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
    }
}
