using System;

namespace Valutech.Data
{
    public class EngineeringDatabaseConnectionData:ServerConnectionData
    {
        public static string DATABASE = "INGENIERIA";

        public new string GetConnectionString()
        {
            return String.Concat(base.GetConnectionString(),"Database=",DATABASE,";");
        }
    }
}