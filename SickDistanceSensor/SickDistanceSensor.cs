using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using AutoFindComLib;
using System.Threading;

namespace SickDistanceSensor
{
    public delegate void MeasurementChangedHandler(string value);
    public delegate void DisplayStateUpdatedHandler(bool on);

    public class SickDistanceSensor
    {
        #region Objects

        private SerialTransport st;
        private CmdMessenger cmdMessenger;
        private AutoFindCom commFinder = new AutoFindCom();
        private Thread measurementThread;

        #endregion

        #region Events

        public event MeasurementChangedHandler MeasurementChanged;
        public event DisplayStateUpdatedHandler DisplayStateUpdated;

        #endregion

        #region Variables

        private string foundComm = String.Empty;
        private bool connected = false;
        private bool displayOn = false;
        private bool measuring = false;

        #endregion

        #region Enums (Commands)

        enum Command
        {
            kMeasurement,
            kOutOfRange,
            kError,
            kSetZero,
            kSetKeyLock,
            kReleaseKeyLock,
            kSetDisplayOn,
            kSetDisplayOff,
            kRequestDisplayState,
            kDisplayStateResp,
            kRequestMeasurement
        };

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SickDistanceSensor()
        {
            foundComm = commFinder.GetCommPort();
            measurementThread = new Thread(new ThreadStart(MeasurementThreadMethod));
        }

        /// <summary>
        /// Measurement thread method, it constantly send the request measurement command
        /// </summary>
        private void MeasurementThreadMethod()
        {
            while (true)
            {
                if (measuring)
                {
                    this.cmdMessenger.SendCommand(new SendCommand((int)Command.kRequestMeasurement));
                    Thread.Sleep(150);
                }
            }
        }

        public void Connect()
        {
            connected = true;
            if(st!= null) st.Dispose();
            st = new SerialTransport();
            st.CurrentSerialSettings.PortName = foundComm;
            st.CurrentSerialSettings.BaudRate = 9600;
            st.CurrentSerialSettings.DtrEnable = false;
            if (cmdMessenger != null) cmdMessenger.Dispose();
            cmdMessenger = new CmdMessenger(st);
            cmdMessenger.BoardType = BoardType.Bit16;
            AttachCommandCallBacks();
            cmdMessenger.StartListening();
            measurementThread = new Thread(new ThreadStart(MeasurementThreadMethod));
            measurementThread.Start();
        }

        public void Disconnect()
        {
            if (cmdMessenger != null)
            {
                cmdMessenger.StopListening();
                cmdMessenger.Dispose();
            }
            if (st != null)
            {
                st.Close();
                st.Kill();
                st.Dispose();
            }
            measurementThread.Abort();
            connected = false;
        }

        public void StartMeasuring()
        {
            measuring = true;
        }

        public void StopMeasuring()
        {
            measuring = false;
        }

        private void AttachCommandCallBacks()
        {
            cmdMessenger.Attach((int)Command.kMeasurement, OnMeasurement);
            cmdMessenger.Attach((int)Command.kOutOfRange, OnOutOfRange);
            cmdMessenger.Attach((int)Command.kDisplayStateResp, OnDisplayStateResp);
        }

        private void OnDisplayStateResp(ReceivedCommand command)
        {
            displayOn = command.ReadBoolArg();
            if (DisplayStateUpdated != null) DisplayStateUpdated(displayOn);
        }

        private void OnMeasurement(ReceivedCommand command)
        {
            string value = command.ReadStringArg();
            if (value != String.Empty)
            {
                if (value[0] == '-')
                {
                    value = value.Substring(1);
                    if (value.Length > 3)
                    {
                        value = "-" + value.Replace(value.Substring(value.Length - 3, 3), String.Empty) + "." + value.Substring(value.Length - 3, 3);
                    }
                    else if (value.Length == 3)
                    {
                        value = "-0." + value;
                    }
                    else if (value.Length == 2)
                    {
                        value = "-0.0" + value;
                    }
                    else if (value.Length == 1)
                    {
                        value = "-0.00" + value;
                    }
                }
                else
                {
                    if (value.Length > 3)
                    {
                        value = value.Replace(value.Substring(value.Length - 3, 3), String.Empty) + "." + value.Substring(value.Length - 3, 3);
                    }
                    else if (value == "0")
                    {
                        value = "0.000";
                    }
                    else if (value.Length == 3)
                    {
                        value = "0." + value;
                    }
                    else if (value.Length == 2)
                    {
                        value = "0.0" + value;
                    }
                    else if (value.Length == 1)
                    {
                        value = "0.00" + value;
                    }
                }                
            }
            if (MeasurementChanged != null) MeasurementChanged(value);
        }

        private void OnOutOfRange(ReceivedCommand command)
        {
            string value = "---";
            if (MeasurementChanged != null) MeasurementChanged(value);
        }

        public void SetZero()
        {
            if (connected)
            {
                this.cmdMessenger.SendCommand(new SendCommand((int) Command.kSetZero));
            }
        }

        public void SetDisplay(Boolean on)
        {
            if (connected)
            {
                if (on)
                {
                    this.cmdMessenger.SendCommand(new SendCommand((int)Command.kSetDisplayOn));
                }
                else
                {
                    this.cmdMessenger.SendCommand(new SendCommand((int)Command.kSetDisplayOff));
                }
            }
        }

        public void RequestDisplayState()
        {
            if (connected)
            {
                this.cmdMessenger.SendCommand(new SendCommand((int)Command.kRequestDisplayState));
            }
        }

        public void SetLock(bool locked)
        {
            if (connected)
            {
                if (locked)
                {
                    this.cmdMessenger.SendCommand(new SendCommand((int)Command.kSetKeyLock));
                }
                else
                {
                    this.cmdMessenger.SendCommand(new SendCommand((int)Command.kReleaseKeyLock));
                }
            }
        }

        public string CommPort
        {
            get
            {
                foundComm = commFinder.GetCommPort();
                return this.foundComm;
            }
        }

        public bool Connected
        {
            get
            {
                return this.connected;
            }
        }

        public bool DisplayOn
        {
            get
            {
                return this.displayOn;
            }
        }
    }
}
