using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
using System.Data.SqlClient;
using Valutech.Data;
using Newtonsoft.Json;
using System.Collections;

namespace Valutech.Electrox
{
    public delegate void LaserEquipmentOnlineStatusChangedHandler(LaserEquipment laserEquipment);

    /// <summary>
    /// Manages the external data handle of the laser equipments list with the database
    /// </summary>
    public class LaserEquipmentListManager
    {
        #region Constants

        private const string TABLE_NAME = "ElectroxLaserEquipments";

        private const string LASER_LIST_TAG = "LaserList";

        private const string LASER_EQUIPMENT_TAG = "LaserEquipment";

        private const string LASER_EQUIPMENT_PLANT_TAG = "Plant";

        private const string LASER_EQUIPMENT_AREA_TAG = "Area";

        private const string LASER_EQUIPMENT_ID_TAG = "Id";

        private const string LASER_EQUIPMENT_NAME_TAG = "Name";

        private const string LASER_EQUIPMENT_IP_TAG = "Ip";

        private const string LASER_EQUIPMENT_DSP_TAG = "Dsp";

        private const string LASER_EQUIPMENT_LENS_TAG = "Lens";

        private const string LASER_EQUIPMENT_LENSAMP_TAG = "LensAmp";

        private const string LASER_EQUIPMENT_XCOMPENSATION_TAG = "XCompensation";

        private const string LASER_EQUIPMENT_YCOMPENSATION_TAG = "YCompensation";

        private const string LASER_EQUIPMENT_DSP_FILE_TAG = "DspFile";

        private const string LASER_EQUIPMENT_MRU_1_TAG = "Mru1";

        private const string LASER_EQUIPMENT_MRU_2_TAG = "Mru2";

        private const string LASER_EQUIPMENT_MRU_3_TAG = "Mru3";

        private const string LASER_EQUIPMENT_MRU_4_TAG = "Mru4";

        private const string LASER_EQUIPMENT_MRU_5_TAG = "Mru5";

        private const string LASER_EQUIPMENT_MRU_6_TAG = "Mru6";

        private const string LASER_EQUIPMENT_MRU_7_TAG = "Mru7";

        private const string LASER_EQUIPMENT_MRU_8_TAG = "Mru8";

        private const string LASER_EQUIPMENT_MRU_9_TAG = "Mru9";

        #endregion

        EngineeringDatabaseConnectionData connectionData = new EngineeringDatabaseConnectionData();
        
        /// <summary>
        /// Unique instance of the class
        /// </summary>
        private static LaserEquipmentListManager instance;

        /// <summary>
        /// List of laser equipments
        /// </summary>
        private List<LaserEquipment> laserEquipments = new List<LaserEquipment>();

        private bool loaded = false;

        public event LaserEquipmentOnlineStatusChangedHandler LaserEquipmentOnlineStatusChanged;

        /// <summary>
        /// Gets the unique instance of the class, created if is first time call
        /// </summary>
        /// <returns></returns>
        public static LaserEquipmentListManager GetInstance()
        {            
            if (instance == null) instance = new LaserEquipmentListManager();
            return instance;
        }

        private LaserEquipmentListManager() { }

        #region Database interaction Methods

        /// <summary>
        /// Load the data from the database
        /// </summary>
        /// <returns>True if the data was loaded successfully</returns>        
        /// <exception cref="FileNotFoundException"></exception>
        public bool LoadData()
        {
            this.laserEquipments = new List<LaserEquipment>();
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(@"SELECT id,name, facility, area, ip, dsp_name, dsp_file, lens_data, mru_data, programs_data, settings_data FROM " + TABLE_NAME + " ORDER BY name ASC", connection);
                try
                {
                    SqlDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        LaserEquipment laser = new LaserEquipment();
                        laser.Id = reader.GetInt32(0).ToString();
                        laser.Name = reader.GetString(1);
                        laser.PlantNumber = (int) reader.GetValue(2);
                        laser.Area = reader.GetString(3);
                        laser.Ip = reader.GetString(4);
                        laser.Dsp = reader.GetString(5);
                        laser.DspFile = reader.GetString(6);
                        laser.lensData = (LensData) JsonConvert.DeserializeObject<LensData>(reader.GetString(7));
                        laser.mruData = (MruData)JsonConvert.DeserializeObject<MruData>(reader.GetString(8));                        
                        laserEquipments.Add(laser);
                    }
                    reader.Close();
                    connection.Close();
                    loaded = true;
                    return true;
                }
                catch (SqlException)
                {
                }                
            }
            return false;
        }

        /// <summary>
        /// Adds the parameters for the query
        /// </summary>
        /// <param name="le"></param>
        /// <param name="command"></param>
        private void AddParameters(LaserEquipment le, SqlCommand command)
        {
            try
            {
                command.Parameters.AddWithValue("@name", le.Name);
                command.Parameters.AddWithValue("@facility", le.PlantNumber);
                command.Parameters.AddWithValue("@area", le.Area);
                command.Parameters.AddWithValue("@ip", le.Ip);
                command.Parameters.AddWithValue("@dsp_name", le.Dsp);
                command.Parameters.AddWithValue("@dsp_file", le.DspFile);
                command.Parameters.AddWithValue("@lens_data", JsonConvert.SerializeObject(le.lensData));
                command.Parameters.AddWithValue("@mru_data", JsonConvert.SerializeObject(le.mruData));
                command.Parameters.AddWithValue("@programs_data", string.Empty);
                command.Parameters.AddWithValue("@settings_data", string.Empty);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Adds a new registry with the laser equipment data information
        /// </summary>
        /// <param name="le"></param>
        public void AddLaserEquipment(LaserEquipment le)
        {
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand insertCommand = new SqlCommand(@"INSERT INTO " + TABLE_NAME + " (name, facility, area, ip, dsp_name, dsp_file, lens_data, mru_data, programs_data, settings_data) output INSERTED.ID VALUES (@name, @facility, @area, @ip, @dsp_name, @dsp_file, @lens_data, @mru_data,@programs_data,@settings_data);SELECT SCOPE_IDENTITY()", connection);
                AddParameters(le, insertCommand);
                try
                {
                    int newId = (int)insertCommand.ExecuteScalar();
                    le.Id = newId.ToString();
                    laserEquipments.Add(le);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Sql Exception:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Delete laser equipment data from the database
        /// </summary>
        /// <param name="le"></param>
        public void DeleteLaserEquipment(LaserEquipment le)
        {
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand deleteCommand = new SqlCommand(@"DELETE FROM " + TABLE_NAME + " WHERE id=@id;", connection);
                deleteCommand.Parameters.AddWithValue("@id", le.Id);
                AddParameters(le, deleteCommand);
                try
                {
                    deleteCommand.ExecuteScalar();
                    LaserEquipment lasertoremove = null;
                    foreach (LaserEquipment l in laserEquipments)
                    {
                        if (l.Id == le.Id) lasertoremove = l;
                    }
                    if(lasertoremove != null)
                    laserEquipments.Remove(lasertoremove);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Sql Exception:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Updates the laser equipment information in the database
        /// </summary>
        /// <param name="laser">Laser information to be updated</param>
        public void SaveLaserSelectionData(LaserEquipment laser)
        {
            using (SqlConnection connection = new SqlConnection(connectionData.GetConnectionString()))
            {
                connection.Open();
                SqlCommand updateCommand = new SqlCommand(@"UPDATE " + TABLE_NAME + " SET name=@name, facility=@facility, area=@area, ip=@ip, dsp_name=@dsp_name, dsp_file=@dsp_file, lens_data=@lens_data, mru_data=@mru_data, programs_data=@programs_data, settings_data=@settings_data WHERE id=@id;", connection);
                updateCommand.Parameters.AddWithValue("@id", laser.Id);
                AddParameters(laser, updateCommand);
                try
                {
                    updateCommand.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Sql Exception:" + ex.ToString());
                }
            }
        }

        #endregion

        void le_OnlineStatusChanged(LaserEquipment sender, bool online)
        {
            if(LaserEquipmentOnlineStatusChanged != null) LaserEquipmentOnlineStatusChanged(sender);
        }

        /// <summary>
        /// Returns the laser equipment list
        /// </summary>
        public List<LaserEquipment> LaserEquipmentList
        {
            get
            {
                if (!loaded) LoadData();
                return laserEquipments;
            }
        }
    }
}
