using System;

namespace Valutech.Data
{
    public class ServerConnectionData
    {
        public static string SERVER = "adm.valuout.com";

        public static string USER = "ing";

        public static string PASS = "067B6zz1sEKIjEJ5I9FB";

        public string GetConnectionString()
        {
            return String.Concat("Server=", SERVER, ";User Id=",USER,";Password=",PASS,";");
        }
    }
}