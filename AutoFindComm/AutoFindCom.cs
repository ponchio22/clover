using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace AutoFindComLib
{    
    public class AutoFindCom
    {
        #region Hardware Enum
        /// <summary>
        /// Enumeration of Win32 API
        /// </summary>
        public enum HardwareEnum
        {
            // Hardware
            Win32_Processor, // The CPU processor
            Win32_PhysicalMemory, // Physical memory
            Win32_Keyboard, // Keyboard
            Win32_PointingDevice, // Input devices, including the mouse. 
            Win32_FloppyDrive, // Floppy disk driver
            Win32_DiskDrive, // Hard disk drive
            Win32_CDROMDrive, // Optical disc drive
            Win32_BaseBoard, // A main board
            Win32_BIOS, // BIOS chip
            Win32_ParallelPort, // Parallel port
            Win32_SerialPort, // Serial port
            Win32_SerialPortConfiguration, // Serial port configuration
            Win32_SoundDevice, // Multimedia settings, generally refers to the sound card. 
            Win32_SystemSlot, // The motherboard slot (ISA & PCI & AGP)
            Win32_USBController, // USB controller
            Win32_NetworkAdapter, // The network adapter
            Win32_NetworkAdapterConfiguration, // Network adapter settings
            Win32_Printer, // The printer
            Win32_PrinterConfiguration, // Printer settings
            Win32_PrintJob, // Printer task
            Win32_TCPIPPrinterPort, // The printer port
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM port
            Win32_DesktopMonitor, // Monitor
            Win32_DisplayConfiguration, // Video card
            Win32_DisplayControllerConfiguration, // Graphics settings
            Win32_VideoController, // The card details. 
            Win32_VideoSettings, // The card supports the display mode. 

            // Operating system
            Win32_TimeZone, // Time zone
            Win32_SystemDriver, // The driver
            Win32_DiskPartition, // The disk partition
            Win32_LogicalDisk, // The logical disk
            Win32_LogicalDiskToPartition, // The logical disk partition and location. 
            Win32_LogicalMemoryConfiguration, // The logical memory allocation
            Win32_PageFile, // The system page file information
            Win32_PageFileSetting, // Page file set
            Win32_BootConfiguration, // System startup configuration
            Win32_ComputerSystem, // Computer information briefly
            Win32_OperatingSystem, // Operating system information
            Win32_StartupCommand, // The system automatically start a program
            Win32_Service, // System installation service
            Win32_Group, // System management group
            Win32_GroupUser, // System account
            Win32_UserAccount, // The user account
            Win32_Process, // System process
            Win32_Thread, // A system thread
            Win32_Share, // Share
            Win32_NetworkClient, // Installed network client
            Win32_NetworkProtocol, // Installed network protocol
            Win32_PnPEntity,//all device
        }

        #endregion
        
        private string[] ss = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");

        private string _commPort = String.Empty;

        public AutoFindCom()
        {

        }

        /// <summary>
        /// Defines the comport to be used
        /// </summary>
        public string GetCommPort()
        {
            string foundPort = String.Empty;
            string[] portnames = SerialPort.GetPortNames();
            var tList = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");
            int i = 0;
            bool found = false;
            foreach (string s in tList)
            {
                if ((s.IndexOf("Arduino") > -1 || s.IndexOf("Prolific") > -1) && !found)
                {
                    Regex reg = new Regex(@"COM(\d+)");
                    string com = reg.Matches(s)[0].ToString();
                    foundPort = com;
                }
                i++;
            }

            if (foundPort != String.Empty)
            {
                if (_commPort != foundPort)
                {
                    _commPort = foundPort;
                }

            }
            else
            {
                _commPort = String.Empty;
            }
            return _commPort;
        }

        public string CommPort
        {
            get
            {
                return this._commPort;
            }
        }

        /// <summary>
        /// WMI hardware information
        /// </summary>
        /// <param name="hardType"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        private static string[] MulGetHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value.ToString().Contains("COM"))
                        {
                            string s = hardInfo.Properties[propKey].Value.ToString();
                            strs.Add(s);
                        }

                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }
    }
}
