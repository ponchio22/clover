using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipeClient;

namespace Valutech.UserManagment
{
    public static class ValutechUserLogStrings
    {
        public static string PIPE_NAME = @"\\.\pipe\logPipe";
        public static string LOGIN = "login";
        public static char SEPARATOR = '|';
        public static string LOG_STATUS = "logStatus";
        public static string DOMAIN = "valuout.com";
        public static string LOGOUT = "logout";
        public static string RESET_INACTIVITY = "resetInactivity";
        public static string LOG_STATUS_RESPONSE = "loggedStatus";
        public static string LOGGED_OUT = "loggedout";
        public static string LOGGED_IN = "loggedin";
    }
}