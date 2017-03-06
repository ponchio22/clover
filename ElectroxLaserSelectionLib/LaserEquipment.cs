using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Valutech.Electrox
{
    public delegate void LaserEquipmentInfoChangedHandler(LaserEquipment laser);

    public delegate void LaserEquipmentPortBusyHandler(LaserEquipment laser);
        
    public class LaserEquipment
    {
        public string Id;
        public string Name = string.Empty;
        public string Plant;
        public string Area;
        public string Dsp = string.Empty;
        public string DspFile = string.Empty;

        private MruData _mruData = new MruData();
        private LensData _lensData = new LensData();

        private string _hardware;

        private string ip = string.Empty;
        private bool _online;
        private ArrayList _programs = new ArrayList();

        public event LaserEquipmentInfoChangedHandler InfoChanged;
        public event LaserEquipmentPortBusyHandler PortBusy;

        private TcpClient tcpclnt;

        private bool locked = false;

        public LaserEquipment() { }
        
        public void LoadPrograms()
        {
            Thread thread = new Thread(AsyncLoadPrograms);
            thread.Start();
        }

        #region Ethernet Communication

        public void LoadCurrentProgram()
        {
            try
            {
                this.CreateConnection();
                this.CloseConnection();
            }
            catch
            {
            }
        }

        public void SelectProgram(string program)
        {
            try
            {
                this.CreateConnection();
                locked = false;
                //if (locked) WriteData(tcpclnt.GetStream(), "DRYRUN " + ((locked) ? "1" : "-1") + ((char)13));
                //WriteData(tcpclnt.GetStream(), ">SPGM " + program.ToUpper() + ((char)13));
                this.CloseConnection();
            }
            catch
            {
            }
        }

        public void TuneGalvos()
        {
            try
            {
                this.CreateConnection();
                WriteData(tcpclnt.GetStream(), "TUNEGALVOS" + ((char)13));
                this.CloseConnection();
            }
            catch
            {
            }
        }

        public void Lock(bool value)
        {
            try
            {
                this.locked = value;
                this.CreateConnection();
                //WriteData(tcpclnt.GetStream(), "DRYRUN " + ((value)? "1":"-1") + ((char)13));
                this.CloseConnection();
            }
            catch
            {
            }
        }

        private void CreateConnection()
        {
            tcpclnt = new TcpClient();
            tcpclnt.Connect(this.ip, 4000);
            tcpclnt.ReceiveTimeout = 500;
        }

        private void CloseConnection()
        {
            tcpclnt.Close();
        }

        private string ReadData(NetworkStream network)
        {
            byte[] bReads = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            int ReadAmount = 0;
            while (network.DataAvailable)
            {
                ReadAmount = network.Read(bReads, 0, bReads.Length);
                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(bReads, 0, ReadAmount));
            }
            return myCompleteMessage.ToString();
        }

        static void WriteData(NetworkStream stream, string cmd)
        {
            stream.Write(Encoding.UTF8.GetBytes(cmd), 0,Encoding.UTF8.GetBytes(cmd).Length);
        }

        public void AsyncLoadPrograms()
        {
            var connectionWatch = Stopwatch.StartNew();
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(this.ip, 4000);
                tcpclnt.ReceiveTimeout = 500;
                this._online = true;
                this._programs.Clear();

                //read stat of the machine
                WriteData(tcpclnt.GetStream(), ((char)13) + ">STAT" + ((char)13));
                bool listening = true;
                var watch = Stopwatch.StartNew();
                while (listening)
                {
                    NetworkStream strm = tcpclnt.GetStream();
                    string data = ReadData(strm);
                    if (data != string.Empty)
                    {
                        listening = false;                        
                    }
                    if (watch.ElapsedMilliseconds > 500)
                    {
                        listening = false;
                        throw new SocketException();                        
                    }
                }

                //Read programs
                WriteData(tcpclnt.GetStream(), ((char)13) + "PROGS" + ((char)13));

                listening = true;
                watch = Stopwatch.StartNew();
                while (listening)
                {
                    NetworkStream strm = tcpclnt.GetStream();
                    string data = ReadData(strm);
                    if (data != string.Empty)
                    {
                        string[] progs = data.Split((char)13);
                        foreach (string prog in progs)
                        {
                            if (prog != string.Empty && prog != "End")
                                this._programs.Add(new LaserProgram().ProcessLaserOutputString(prog));
                            else if (prog == "End")
                                listening = false;
                        }
                        watch.Restart();
                    }
                    if (watch.ElapsedMilliseconds > 500) listening = false;
                }

                WriteData(tcpclnt.GetStream(), "ROMVER" + ((char)13));

                listening = true;
                watch = Stopwatch.StartNew();
                while (listening)
                {
                    NetworkStream strm = tcpclnt.GetStream();
                    string data = ReadData(strm);
                    if (data != string.Empty)
                    {
                        this._hardware = data.Split(' ')[data.Split(' ').Length - 1];
                        watch.Restart();
                    }
                    if (watch.ElapsedMilliseconds > 500) listening = false;
                }
                
                if (InfoChanged != null) InfoChanged(this);
                tcpclnt.Close();
                
            }            
            catch (SocketException)
            {
                if (PortBusy != null) PortBusy(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        #endregion

        #region Object Getters/Setters

        /// <summary>
        /// Ip Address of the Electrox Laser Machine
        /// </summary>        
        public string Ip
        {
            set
            {
                this.ip = value;
            }
            get
            {
                return this.ip;
            }
        }
                
        public int Port
        {
            get
            {
                return 4000;
            }
        }

        public bool Online
        {
            get
            {
                return this._online;
            }
        }

        
        public ArrayList Programs
        {
            get
            {
                return this._programs;
            }
        }

        public string Hardware
        {
            get
            {
                return this._hardware;
            }
        }

        public int PlantNumber
        {
            get
            {
                if (this.Plant != String.Empty)
                {
                    return int.Parse(this.Plant.Substring(this.Plant.Length - 1, 1));
                }
                else
                {
                    return 0;
                }
            }
            set 
            {
                if (value != 0)
                {
                    this.Plant = "Planta " + value.ToString();
                }
                else
                {
                    this.Plant = string.Empty;
                }
            }
        }

        public MruData mruData
        {
            get
            {
                return _mruData;
            }
            set
            {
                this._mruData = value;
            }
        }

        public LensData lensData
        {
            get
            {
                return _lensData;
            }
            set
            {
                this._lensData = value;
            }
        }

        #endregion

        #region MRU Getters/Setters

        public string Mru1
        {
            set
            {
                _mruData.Mru1 = value;
            }
            get
            {
                return _mruData.Mru1;
            }
        }

        public string Mru2
        {
            set
            {
                _mruData.Mru2 = value;
            }
            get
            {
                return _mruData.Mru2;
            }
        }

        public string Mru3
        {
            set
            {
                _mruData.Mru3 = value;
            }
            get
            {
                return _mruData.Mru3;
            }
        }

        public string Mru4
        {
            set
            {
                _mruData.Mru4 = value;
            }
            get
            {
                return _mruData.Mru4;
            }
        }

        public string Mru5
        {
            set
            {
                _mruData.Mru5 = value;
            }
            get
            {
                return _mruData.Mru5;
            }
        }

        public string Mru6
        {
            set
            {
                _mruData.Mru6 = value;
            }
            get
            {
                return _mruData.Mru6;
            }
        }

        public string Mru7
        {
            set
            {
                _mruData.Mru7 = value;
            }
            get
            {
                return _mruData.Mru7;
            }
        }

        public string Mru8
        {
            set
            {
                _mruData.Mru8 = value;
            }
            get
            {
                return _mruData.Mru8;
            }
        }

        public string Mru9
        {
            set
            {
                _mruData.Mru9 = value;
            }
            get
            {
                return _mruData.Mru9;
            }
        }

        #endregion

        #region Lens Data Getters/Setters

        public string Lens
        {
            set
            {
                _lensData.Lens = value;
            }
            get
            {
                return _lensData.Lens;
            }
        }

        public string LensAmp
        {
            set
            {
                _lensData.LensAmp = value;
            }
            get
            {
                return _lensData.LensAmp;
            }
        }

        public string XCompensation
        {
            set
            {
                _lensData.XCompensation = value;
            }
            get
            {
                return _lensData.XCompensation;
            }
        }

        public string YCompensation
        {
            set
            {
                _lensData.YCompensation = value;
            }
            get
            {
                return _lensData.YCompensation;
            }
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class MruData
    {
        public string Mru1 = string.Empty;
        public string Mru2 = string.Empty;
        public string Mru3 = string.Empty;
        public string Mru4 = string.Empty;
        public string Mru5 = string.Empty;
        public string Mru6 = string.Empty;
        public string Mru7 = string.Empty;
        public string Mru8 = string.Empty;
        public string Mru9 = string.Empty;
    }

    public class LensData
    {
        public string Lens = string.Empty;
        public string LensAmp = string.Empty;
        public string XCompensation = string.Empty;
        public string YCompensation = string.Empty;
    }
}
