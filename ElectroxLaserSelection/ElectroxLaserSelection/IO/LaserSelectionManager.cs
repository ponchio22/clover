using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Valutech.Electrox
{
    class LaserSelectionManager
    {
        private LaserEquipmentListManager manager = LaserEquipmentListManager.GetInstance();

        private SettingsManager settings = SettingsManager.GetInstance();

        private string REGISTRY_PATH = (System.Environment.Is64BitOperatingSystem) ? @"SOFTWARE\Wow6432Node\" : @"SOFTWARE\";
        private const string SCRIBA_KEY = @"Electrox\Scriba\";
        private const string GLOBAL_PREFS_REGISTRY_KEY = @"Electrox\Scriba\Global Prefs";
        private const string DSP_PARAMETERS_REGISTRY_KEY = @"Electrox\Scriba\DSP Parameters";
        private const string MRU_REGISTRY_KEY = @"Electrox\Scriba\MRU";
        private const string INI_FILE_LOCATION_KEY = @"Electrox\Scriba\IniFileLocation";
        private const string FONT_AUTO_DOWNLOAD_REGISTRY_KEY = @"Electrox\Scriba\FontAutoDownload";
        private const string COMM_PORT_KEY = "CommPort";
        private const string ETHERNET_IP_KEY = "Ethernet IP Address";
        private const string ETHERNET_PORT_KEY = "Ethernet Port";
        private const string LAST_USED_KEY = "Last Used";
        private const string MARK_INI_FILE_KEY = "MarkerIniDir";
        private const string MRU_1_KEY = "MRU1";
        private const string MRU_2_KEY = "MRU2";
        private const string MRU_3_KEY = "MRU3";
        private const string MRU_4_KEY = "MRU4";
        private const string MRU_5_KEY = "MRU5";
        private const string MRU_6_KEY = "MRU6";
        private const string MRU_7_KEY = "MRU7";
        private const string MRU_8_KEY = "MRU8";
        private const string MRU_9_KEY = "MRU9";
        private const string NUM_FONTS_KEY = "NumFonts";
        private const string FONT_TO_DOWNLOAD_KEY = "FontToDownload";
        private const string FONT_CHECKED_KEY = "FontChecked";        
        private const string INSTALL_DIR_KEY = "InstallDir";
        private const string PROCESS_NAME = "Scriba3";
        private const string PROCESS_FILE = PROCESS_NAME + ".exe";
        private const string DSP_PARAMETERS_FOLDER = "DSP Parameters";
        
        /// <summary>
        /// Select the given laser equipment, change the registry values needed to handle this
        /// </summary>
        /// <param name="equipmentName">Name of the laser equipment</param>
        public void SelectLaserEquipment(string equipmentName)
        {   
            List<LaserEquipment> list = manager.LaserEquipmentList;
            LaserEquipment equipment = null;
            foreach (LaserEquipment item in list)
            {
                if (item.Name == equipmentName)
                {
                    equipment = item;
                }
            }
            if (equipment != null)
            {
                //Declare objects 
                RegistryKey regBase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                RegistryKey globalPrefs = regBase.OpenSubKey(REGISTRY_PATH + GLOBAL_PREFS_REGISTRY_KEY, true);
                try
                {
                    regBase.CreateSubKey(REGISTRY_PATH + DSP_PARAMETERS_REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                catch { }
                RegistryKey dspParam = regBase.OpenSubKey(REGISTRY_PATH + DSP_PARAMETERS_REGISTRY_KEY, true);
                try
                {
                    regBase.CreateSubKey(REGISTRY_PATH + MRU_REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                catch { }
                RegistryKey mru = regBase.OpenSubKey(REGISTRY_PATH + MRU_REGISTRY_KEY, true);
                RegistryKey fontAuto = regBase.OpenSubKey(REGISTRY_PATH + FONT_AUTO_DOWNLOAD_REGISTRY_KEY, true);
                RegistryKey scribaKey = regBase.OpenSubKey(REGISTRY_PATH + SCRIBA_KEY);
                RegistryKey iniFilesKey = regBase.OpenSubKey(REGISTRY_PATH + INI_FILE_LOCATION_KEY, true);
                string installDir = scribaKey.GetValue(INSTALL_DIR_KEY).ToString();
                string markIniFile = iniFilesKey.GetValue(MARK_INI_FILE_KEY).ToString();

                //Close the program if open
                Process[] processes = Process.GetProcessesByName(PROCESS_NAME);
                if (processes.Count() > 0) processes[0].Kill();

                //Copy dsp settings
                if (File.Exists(equipment.DspFile))
                {
                    string[] files = Directory.GetFiles(Path.Combine(installDir, DSP_PARAMETERS_FOLDER));
                    foreach (string file in files) File.Delete(file);
                    File.Copy(equipment.DspFile, Path.Combine(installDir, DSP_PARAMETERS_FOLDER, Path.GetFileName(equipment.DspFile)), true);
                }

                //Write values in registry                
                globalPrefs.SetValue(ETHERNET_IP_KEY, equipment.Ip);
                globalPrefs.SetValue(COMM_PORT_KEY, 12);
                globalPrefs.SetValue(ETHERNET_PORT_KEY, 4000);
                dspParam.SetValue(LAST_USED_KEY, equipment.Dsp);
                mru.SetValue(MRU_1_KEY, equipment.Mru1);
                mru.SetValue(MRU_2_KEY, equipment.Mru2);
                mru.SetValue(MRU_3_KEY, equipment.Mru3);
                mru.SetValue(MRU_4_KEY, equipment.Mru4);
                mru.SetValue(MRU_5_KEY, equipment.Mru5);
                mru.SetValue(MRU_6_KEY, equipment.Mru6);
                mru.SetValue(MRU_7_KEY, equipment.Mru7);
                mru.SetValue(MRU_8_KEY, equipment.Mru8);
                mru.SetValue(MRU_9_KEY, equipment.Mru9);

                //Update font autodownload settings
                List<string> fonts = FontAutoDownloadManager.GetInstance().Fonts;
                fontAuto.SetValue(NUM_FONTS_KEY, fonts.Count());
                int count = 1;
                foreach (string font in fonts)
                {
                    fontAuto.SetValue(FONT_CHECKED_KEY + count.ToString(), 1);
                    fontAuto.SetValue(FONT_TO_DOWNLOAD_KEY + count.ToString(), font);
                    count++;
                }

                //Update lens setup
                if (File.Exists(settings.MarkerIniFileSource))
                {
                    File.Copy(settings.MarkerIniFileSource, markIniFile, true);
                    MarkerIniFile iniFile = new MarkerIniFile(markIniFile);
                    iniFile.LoadLensSetups();
                    List<LensSetup> lensSetups = iniFile.LensSetups;
                    foreach (LensSetup setup in lensSetups)
                    {
                        if (setup.Name == equipment.Lens)
                        {
                            setup.LensAmplitude = float.Parse(equipment.LensAmp);
                            setup.XCompensation = float.Parse(equipment.XCompensation);
                            setup.YCompensation = float.Parse(equipment.YCompensation);
                            iniFile.SetSetupSelected(setup);
                        }
                    }
                }

                //Open program                                 
                Process.Start(new ProcessStartInfo(Path.Combine(installDir, PROCESS_FILE)));                
            }
        }
    }
}
