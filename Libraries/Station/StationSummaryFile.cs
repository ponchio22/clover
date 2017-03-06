using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Valutech.Configurations;
using Valutech.Sync;
using Valutech.RF;
using System.Security.Principal;
using System.Data.SqlClient;
using Valutech.Data;
using System.Collections;

namespace Valutech.Station
{
    public class StationSummaryFile
    {

        #region DataSource Information Fields

        private static string informationTag = "Information";
        private static string computerNameTag = "ComputerName";
        private static string ipTag = "Ip";
        private static string userNameTag = "Username";
        private static string isAdminTag = "IsAdmin";
        private static string versionTag = "Version";        
        private static string typeTag = "Type";
        private static string lastUpdateTag = "LastUpdate";
        private static string descriptionTag = "Description";
        private static string serialNumberTag = "SN";        

        #endregion

        #region Field variables which holds the value from the data source

        private string workstation;
        private string ip;
        private string user;
        private bool isAdmin;        
        private string version;
        private string type;
        private string lastUpdate;
        private string description;
        private string equipmentSerialNumber;
        private bool vncAvailable;
        private string carrier;
        private string oem;
        private string model;

        #endregion

        EngineeringDatabaseConnectionData connectionData = new EngineeringDatabaseConnectionData();

        public static string TABLE_NAME = "computers";

        private bool loaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public StationSummaryFile()
        {
        }

        /// <summary>
        /// Constructor for the 
        /// </summary>
        /// <param name="station"></param>
        public StationSummaryFile(string station)
        {
            this.workstation = station;
        }

        public void Save()
        {
            Save(GetMyWorkStation());
        }

        /// <summary>
        /// Gets the date that needs to be saved and save it into the data source
        /// </summary>
        /// <param name="path"></param>
        public void Save(string station)
        {       
            this.workstation = station;
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand insertCommand = new SqlCommand(@"INSERT INTO " + TABLE_NAME + " (computer_name, ip, version, type, last_version_update, description, carrier, OEM, model, [user], has_vnc, test_equipment_sn,is_admin) VALUES (@wks, @ip, @version, @type, @last_version_update, @description, @carrier, @oem, @model, @user, @has_vnc, @test_equipment_sn,@is_admin);", connection);
                SqlCommand updateCommand = new SqlCommand(@"UPDATE " + TABLE_NAME + " SET ip=@ip, version=@version, type=@type, last_version_update=@last_version_update, description=@description, carrier=@carrier, OEM=@oem, model=@model,[user]=@user,has_vnc=@has_vnc,test_equipment_sn=@test_equipment_sn,is_admin=@is_admin WHERE computer_name=@wks", connection);
                AddParameters(insertCommand);
                AddParameters(updateCommand);
                try
                {
                    insertCommand.ExecuteScalar();
                }
                catch (SqlException)
                {
                    try
                    {
                        updateCommand.ExecuteScalar();
                    }
                    catch (SqlException)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Add the parameters for the update and insert queries
        /// </summary>
        /// <param name="command"></param>
        private void AddParameters(SqlCommand command)
        {
            command.Parameters.AddWithValue("@ip", GetIP());
            command.Parameters.AddWithValue("@version", GetVersion());
            command.Parameters.AddWithValue("@description", GetPhysicalLocation());
            command.Parameters.AddWithValue("@user", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            command.Parameters.AddWithValue("@has_vnc", StationConfiguration.GetInstance().StationVNCAvailable);
            command.Parameters.AddWithValue("@type", GetType());
            command.Parameters.AddWithValue("@last_version_update", DateTime.Now);
            command.Parameters.AddWithValue("@carrier", string.Empty);
            command.Parameters.AddWithValue("@oem", string.Empty);
            command.Parameters.AddWithValue("@model", string.Empty);
            command.Parameters.AddWithValue("@test_equipment_sn", Valutech.RF.Agilent.GetSN());
            command.Parameters.AddWithValue("@wks",this.workstation);
            command.Parameters.AddWithValue("@is_admin", new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));            
        }

        /// <summary>
        /// Delete the registry of the current workstation
        /// </summary>
        public void Delete()
        {
            Delete(GetMyWorkStation());
        }

        /// <summary>
        /// Delete the data from the data source
        /// </summary>
        /// <param name="path"></param>
        /// <param name="station"></param>
        public void Delete(string station)
        {
            this.workstation = station;
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand deleteCommand = new SqlCommand(@"DELETE FROM " + TABLE_NAME + " WHERE computer_name=@wks;", connection);
                deleteCommand.Parameters.AddWithValue("@wks", station);                
                try
                {
                    deleteCommand.ExecuteScalar();
                }
                catch (SqlException)
                {                    
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Loads all the data from the current workstation
        /// </summary>
        public ArrayList LoadByType(string type)
        {
            ArrayList result = new ArrayList();
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(@"SELECT computer_name, ip, version, type, last_version_update, description, carrier, OEM, model, [user], has_vnc, test_equipment_sn,is_admin FROM " + TABLE_NAME + " WHERE type=@type;", connection);
                selectCommand.Parameters.AddWithValue("@type", type);
                
                try
                {
                    SqlDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        StationSummaryFile summaryItem = new StationSummaryFile();
                        summaryItem.Workstation = reader.GetString(0);
                        summaryItem.ip = reader.GetString(1);
                        summaryItem.version = reader.GetString(2);
                        summaryItem.type = reader.GetString(3);
                        summaryItem.lastUpdate = reader.GetDateTime(4).ToString();
                        summaryItem.description = reader.GetString(5);
                        summaryItem.carrier = reader.GetString(6);
                        summaryItem.oem = reader.GetString(7);
                        summaryItem.model = reader.GetString(8);
                        summaryItem.user = reader.GetString(9);
                        summaryItem.vncAvailable = reader.GetBoolean(10);
                        summaryItem.equipmentSerialNumber = reader.GetString(11);
                        summaryItem.isAdmin = reader.GetBoolean(12);
                        result.Add(summaryItem);
                    }
                    reader.Close();
                }
                catch (SqlException)
                {
                }
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Loads all the data from the current workstation
        /// </summary>
        public ArrayList LoadAll()
        {
            ArrayList result = new ArrayList();
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(@"SELECT computer_name, ip, version, type, last_version_update, description, carrier, OEM, model, [user], has_vnc, test_equipment_sn,is_admin FROM " + TABLE_NAME, connection);                
                try
                {
                    SqlDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        StationSummaryFile summaryItem = new StationSummaryFile();
                        summaryItem.Workstation = reader.GetString(0);
                        summaryItem.ip = reader.GetString(1);
                        summaryItem.version = reader.GetString(2);
                        summaryItem.type = reader.GetString(3);
                        summaryItem.lastUpdate = reader.GetDateTime(4).ToString();
                        summaryItem.description = reader.GetString(5);
                        summaryItem.carrier = reader.GetString(6);
                        summaryItem.oem = reader.GetString(7);
                        summaryItem.model = reader.GetString(8);
                        summaryItem.user = reader.GetString(9);
                        summaryItem.vncAvailable = reader.GetBoolean(10);
                        summaryItem.equipmentSerialNumber = reader.GetString(11);
                        summaryItem.isAdmin = reader.GetBoolean(12);
                        result.Add(summaryItem);
                    }
                    reader.Close();
                }
                catch (SqlException)
                {
                }
                connection.Close();
            }
            return result;
        }

        public void Load()
        {
            Load(GetMyWorkStation());
        }

        /// <summary>
        /// Load the information from the datasource and fill the data into the object properties
        /// </summary>
        public void Load(string station)
        {            
            this.workstation = station;            
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(@"SELECT computer_name, ip, version, type, last_version_update, description, carrier, OEM, model, [user], has_vnc, test_equipment_sn,is_admin FROM "+TABLE_NAME+" WHERE computer_name=@wks;", connection);                
                selectCommand.Parameters.AddWithValue("@wks", this.workstation);
                try
                {
                    SqlDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        this.workstation = reader.GetString(0);
                        this.ip = reader.GetString(1);
                        this.version = reader.GetString(2);
                        this.type = reader.GetString(3);
                        this.lastUpdate = reader.GetDateTime(4).ToString();
                        this.description = reader.GetString(5);
                        this.carrier = reader.GetString(6);
                        this.oem = reader.GetString(7);
                        this.model = reader.GetString(8);
                        this.user = reader.GetString(9);
                        this.vncAvailable = reader.GetBoolean(10);
                        this.equipmentSerialNumber = reader.GetString(11);
                        this.isAdmin = reader.GetBoolean(12);                                                
                    }
                    reader.Close();
                }
                catch (SqlException)
                {
                }
                connection.Close();
            }
        }

        private string GetPhysicalLocation()
        {
            string result = StationConfiguration.GetInstance().StationPhysicalLocation;
            if (result == null) result = string.Empty;
            return result;            
        }

        private string GetMyWorkStation()
        {
            return System.Environment.MachineName;
        }

        private string GetIP()
        {
            //Get ip
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public string GetVersion()
        {
            string path = ValutechUpdaterConfiguration.GetInstance().LocalLocation;
            string version = String.Empty;
            if(path!=null || path!=String.Empty) {
                VersionIniFile versionFile = new VersionIniFile(path);
                version = versionFile.Version;
            }           
            return version;
        }

        private new string GetType()
        {
            string path = ValutechUpdaterConfiguration.GetInstance().UpdatesLocation;
            string type = String.Empty;
            if (path != null || path != String.Empty)
            {
                Regex reg = new Regex(@"[^\\]{1,}$");
                Match match = reg.Match(path.Substring(0, path.Length - 1));
                type = match.ToString();
            }
            return type;
        }

        #region Datasource field names

        public static string InformationTagName
        {
            get
            {
                return StationSummaryFile.informationTag;
            }
        }

        public static string IpTagName
        {
            get
            {
                return StationSummaryFile.ipTag;
            }
        }

        public static string WorkstationTagName
        {
            get
            {
                return StationSummaryFile.computerNameTag;
            }
        }

        public static string LastUpdateTagName
        {
            get
            {
                return StationSummaryFile.lastUpdateTag;
            }
        }

        public static string VersionTagName
        {
            get
            {
                return StationSummaryFile.versionTag;
            }
        }

        public static string TypeTagName
        {
            get
            {
                return StationSummaryFile.typeTag;
            }
        }

        public static string SerialNumberTagName
        {
            get
            {
                return StationSummaryFile.serialNumberTag;
            }
        }

        public static string DescriptionTagName
        {
            get
            {
                return StationSummaryFile.descriptionTag;
            }
        }

        public static string UserNameTagName
        {
            get
            {
                return StationSummaryFile.userNameTag;
            }
        }

        public static string IsAdminTagName
        {
            get
            {
                return StationSummaryFile.isAdminTag;
            }
        }

        #endregion

        #region Fields Getters

        public string Workstation
        {
            set
            {
                loaded = true;
                this.workstation = value;
            }
            get
            {
                if (!loaded) Load();
                return this.workstation;
            }
        }

        public string Ip
        {
            get
            {
                if (!loaded) Load();
                return this.ip;
            }
        }

        public string User
        {
            get
            {
                if (!loaded) Load();
                return this.user;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (!loaded) Load();
                return this.isAdmin;
            }
        }

        public string Version
        {
            get
            {
                if (!loaded) Load();
                return this.version;
            }                
        }

        public string Type
        {
            set
            {
                loaded = true;
                this.type = value;
            }
            get
            {
                if (!loaded) Load();
                return this.type;
            }
        }

        public string LastUpdate
        {
            get
            {
                if (!loaded) Load();
                return this.lastUpdate;
            }
        }

        public string Description
        {
            get
            {
                if (!loaded) Load();
                return this.description;
            }
        }

        public string EquipmentSerialNumber
        {
            get
            {
                if (!loaded) Load();
                return this.equipmentSerialNumber;
            }
        }

        public bool VNCAvailable
        {
            get
            {
                if (!loaded) Load();
                return this.vncAvailable;
            }
        }
        #endregion
    }
}
