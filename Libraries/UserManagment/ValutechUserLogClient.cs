using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipeClient;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using System.Timers;

namespace Valutech.UserManagment
{
    public class ValutechUserLogClient
    {
        public delegate void LoggedInEventHandler(bool succeeded);
        public event LoggedInEventHandler LoggedIn;

        public delegate void LoggedOutEventHandler();
        public event LoggedOutEventHandler LoggedOut;

        public delegate void LogStatusResponseHandler(bool logged);
        public event LogStatusResponseHandler LogStatusResponse;
        
        /// <summary>
        /// Unique instance
        /// </summary>
        private static ValutechUserLogClient instance;

        /// <summary>
        /// Group name
        /// </summary>
        private string groupName = "EngUsers";

        /// <summary>
        /// Current mode
        /// </summary>
        private Modes currentMode;

        /// <summary>
        /// Logged value
        /// </summary>
        private bool logged = false;

        /// <summary>
        /// Timer to handle the inactivity time
        /// </summary>
        private Timer inactivityTimer = new Timer();

        /// <summary>
        /// Current inactivity time
        /// </summary>
        private long inactivityTime = 0;

        /// <summary>
        /// Maximum number of seconds of inactivity
        /// </summary>
        private const long maxInactivityTime = 600;

        private string lastUser;

        private string lastUserGroup;

        private string lastUserGroupDescription;

        /// <summary>
        /// Enums
        /// </summary>
        public enum Modes
        {
            ENGINEERING,
            QC_VALIDATION_TOOL
        }

        /// <summary>
        /// Private Constructor
        /// </summary>
        private ValutechUserLogClient()
        {
            Mode = Modes.ENGINEERING;
            inactivityTimer = new Timer();
            inactivityTimer.Interval = 1000;
            inactivityTimer.Elapsed += new ElapsedEventHandler(inactivityTimer_Elapsed);            
        }

        /// <summary>
        /// Gets the unique instance of the valutech userlog client
        /// </summary>
        /// <returns></returns>
        public static ValutechUserLogClient GetInstance()
        {
            if (instance == null) instance = new ValutechUserLogClient();
            return instance;
        }

        /// <summary>
        /// Setter for the Log Mode (changes the group to login)
        /// </summary>
        public Modes Mode
        {
            set
            {
                this.currentMode = value;
                if (currentMode == Modes.ENGINEERING)
                {
                    groupName = "EngUsers";
                }
                else
                {
                    groupName = "ET_QCStations-Write";
                }
            }
        }

        public bool CheckGroup()
        {
            String cusername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string[] splittedUser = cusername.Split('\\');
            string domain = splittedUser[0].ToLower() + ".com";
            string user = splittedUser[1].Replace(".admin", "");
            return CheckGroup(user,domain);
        }

        public bool CheckGroup(string username)
        {
            string domain = ValutechUserLogStrings.DOMAIN;
            return CheckGroup(username, domain);
        }

        public bool CheckGroup(String username, string domain)
        {
            bool groupFound = false;
            try
            {
                using (var aPrincipalContext = new PrincipalContext(ContextType.Domain, domain, "vto_service", ".B1s-78r06x0]Tl"))
                {
                    var aPrincipal = UserPrincipal.FindByIdentity(aPrincipalContext, username);
                    if (aPrincipal == null) return false;
                    PrincipalSearchResult<Principal> groups = aPrincipal.GetAuthorizationGroups();
                    foreach (Principal group in groups)
                    {
                        if (group.Name == groupName)
                        {
                            groupFound = true;
                            lastUserGroup = group.Name;
                            lastUserGroupDescription = group.Description;
                        }
                    }
                    return groupFound;
                }
            }
            catch (Exception)
            {                
            }
            return false;
        }

        /// <summary>
        /// Validate the login information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(String username, String password)
        {
            lastUser = username;
            bool groupFound = false;
            try
            {
                using (var aPrincipalContext = new PrincipalContext(ContextType.Domain, ValutechUserLogStrings.DOMAIN, username, password))
                {
                    var aPrincipal = UserPrincipal.FindByIdentity(aPrincipalContext, username);
                    if (aPrincipal == null) return false;
                    PrincipalSearchResult<Principal> groups = aPrincipal.GetAuthorizationGroups();
                    foreach (Principal group in groups)
                    {
                        if (group.Name == groupName)
                        {
                            groupFound = true;
                            lastUserGroup = group.Name;
                            lastUserGroupDescription = group.Description;
                        }
                    }
                    if (!groupFound) return false;
                    if (aPrincipalContext.ValidateCredentials(username, password))
                    {
                        StartTimer();
                        logged = true;
                        if (this.LoggedIn != null) this.LoggedIn(true);
                        return true;
                    }
                    else
                    {
                        if (this.LoggedIn != null) this.LoggedIn(false);
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Logout the user from the system
        /// </summary>
        public void Logout()
        {
            logged = false;
            StopTimer();
            if (this.LoggedOut != null) this.LoggedOut();
        }

        /// <summary>
        /// Returns the log status as an event
        /// </summary>
        public void RequestLogStatus()
        {
            if (CheckGroup()) logged = true;
            if (this.LogStatusResponse != null) this.LogStatusResponse(logged);
        }

        /// <summary>
        /// Getter for the logged status
        /// </summary>
        public bool Logged
        {
            get
            {
                return this.logged;
            }
        }

        /// <summary>
        /// Last User used to try to login
        /// </summary>
        public string LastUser
        {
            get
            {
                return this.lastUser;
            }
        }

        /// <summary>
        /// Last user group used
        /// </summary>
        public string LastUserGroup
        {
            get
            {
                return this.lastUserGroup;
            }
        }

        /// <summary>
        /// Last user group description used
        /// </summary>
        public string LastUserGroupDescription
        {
            get
            {
                return this.lastUserGroupDescription;
            }
        }

        #region Auto logout if inactivity time is reached

        private void StartTimer()
        {
            inactivityTime = 0;
            inactivityTimer.Start();
        }

        private void StopTimer()
        {            
            inactivityTimer.Stop();
            inactivityTime = 0;
        }

        /// <summary>
        /// Reset the inactivity time
        /// </summary>
        private void resetTimer()
        {
            inactivityTime = 0;
        }

        /// <summary>
        /// Resets the inactivity time
        /// </summary>
        public void ResetInactivity()
        {
            resetTimer();
            #region Old Code
            /*
            if (UsePipes)
            {
                Connect();
                if (client.Connected)
                {
                    client.SendMessage(ValutechUserLogStrings.RESET_INACTIVITY);
                }
            }
            else
            {
                userLogActivity.resetTimer();
            }
            */
            #endregion            
        }

        /// <summary>
        /// Handle the timer elapsed, counts how many seconds the station logged has been without any activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void inactivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            inactivityTime++;
            if (inactivityTime >= maxInactivityTime)
            {
                Logout();
            }
        }

        #endregion

    }
}