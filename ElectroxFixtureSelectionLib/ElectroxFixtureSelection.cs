using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFindComLib;
using System.Threading;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using Valutech.Electrox;
using ElectroxProgramsManagmentLib;

namespace ElectroxFixtureSelectionLib
{
    public delegate void CommPortChangedHandler(string commPort);

    public delegate void FixtureChangedHandler(int fixture);

    public delegate void ConnectionStatusChangedHandler(bool connected);

    public delegate void FixtureSelectionChangedHandler(ElectroxProgramInfo programInfo);

    public delegate void NoFixtureFoundHandler();

    public delegate void InvalidFixtureHandler();

    public delegate void NoProgramFoundHandler();

    public delegate void NoPartFoundHandler();
    
    public delegate void PartPresenceStatusChangedHandler(bool present);

    /// <summary>
    /// Used as an interface from the hardware and the electrox equipment, automatically looks for, and connects to the fixture manager (Arduino)
    /// if any change in model is found in the hardware sends the commands to the electrox laser equipment to handle that change
    /// </summary>
    public class ElectroxFixtureSelection
    {
        #region Constants

        public const int NO_FIXTURE = 0;

        #endregion

        #region Objects

        private AutoFindCom autoFindComm = new AutoFindCom();

        private CmdMessenger cmdMessenger;

        private SerialTransport st = new SerialTransport();

        private LaserEquipment laser = null;
        
        private Thread connectionThread;

        #endregion

        #region Events

        /// <summary>
        /// Indicates that the Communication Port Changed if no port is found sends an empty string as parameter
        /// </summary>
        public event CommPortChangedHandler CommPortChanged;

        /// <summary>
        /// Indicates that the fixture has changed in the hardware
        /// </summary>
        public event FixtureChangedHandler FixtureChanged;

        /// <summary>
        /// Indicates that the connection status has changed, the parameter returned shows if its connected or disconnected as a boolean
        /// </summary>
        public event ConnectionStatusChangedHandler ConnectionStatusChanged;

        /// <summary>
        /// Indicates that a program change has been sent to the Electrox Laser Equipment
        /// </summary>
        public event FixtureSelectionChangedHandler FixtureSelectionChanged;

        /// <summary>
        /// Triggered when the fixture has been removed or has not been found in the machine
        /// </summary>
        public event NoFixtureFoundHandler NoFixtureFound;

        /// <summary>
        /// Indicates that the fixture in the machine has an invalid code or has not been setup
        /// </summary>
        public event InvalidFixtureHandler InvalidFixture;

        /// <summary>
        /// Triggered when trying to select the fixure program, it is not found in the Electrox Laser Machine
        /// </summary>
        public event NoProgramFoundHandler NoProgramFound;

        /// <summary>
        /// Triggered when the part presence state is toggled by the sensor (part placed or removed) and when theres a request for the status
        /// </summary>
        public event PartPresenceStatusChangedHandler PartPresenceStatusChanged;

        /// <summary>
        /// Triggered when no part was found in the fixture
        /// </summary>
        public event NoPartFoundHandler NoPartFound;

        #endregion
               
        private string _currentCommPort = String.Empty;

        private bool connected = false;

        private int fixture;

        private bool partPresent = false;
        
        private enum Command{
            kModelChanged,
            kRequestModel,
            kRequestPartStatus,
            kPartStatus
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ElectroxFixtureSelection()
        {            
            connectionThread = new Thread(monitorConnection);
            connectionThread.Start();
        }

        #region Getters/Setters

        /// <summary>
        /// Gets the currently used communication port
        /// </summary>
        public string CommPort
        {
            get
            {
                return _currentCommPort;
            }
        }

        /// <summary>
        /// Sets the laser equipment its going to be used to send the program selection and lock commands
        /// </summary>
        public LaserEquipment Laser
        {
            set
            {
                this.laser = value;
            }
            get
            {
                return this.laser;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends a request for the model to the Fixture Manager, the response is given by the FixtureChanged event
        /// </summary>
        public void RequestModel()
        {
            cmdMessenger.SendCommand(new SendCommand((int)Command.kRequestModel));
        }

        /// <summary>
        /// Sends a request for the status of the presence of the part in the fixture if any, the response is given by the PartPresenceStatusChanged event
        /// </summary>
        public void RequestPartPresent()
        {
            cmdMessenger.SendCommand(new SendCommand((int)Command.kRequestPartStatus));
        }

        /// <summary>
        /// Stops the object thread 
        /// </summary>
        public void Stop()
        {
            connectionThread.Abort();
            Disconnect();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Monitors the connection to with the arduino
        /// </summary>
        private void monitorConnection()
        {
            while (true)
            {
                string tempCom = autoFindComm.GetCommPort();
                if (tempCom != _currentCommPort)
                {
                    if (tempCom != String.Empty)
                    {
                        if (!connected)
                        {
                            Connect(tempCom);                            
                        }
                        else
                        {
                            Disconnect();
                            Connect(tempCom);
                        }
                        RequestModel();
                        RequestPartPresent();
                    }
                    else
                    {
                        Disconnect();
                    }
                    _currentCommPort = tempCom;
                    if (CommPortChanged != null) CommPortChanged(_currentCommPort);                    
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Connects to the provided communication port, attached necessary command callbacks and send an event for the connection status change
        /// </summary>
        /// <param name="foundComm">Communication Port to connect to</param>
        private void Connect(string foundComm)
        {
            connected = true;
            if (st != null) st.Dispose();
            st = new SerialTransport();
            st.CurrentSerialSettings.PortName = foundComm;
            st.CurrentSerialSettings.BaudRate = 9600;
            st.CurrentSerialSettings.DtrEnable = false;
            if (cmdMessenger != null) cmdMessenger.Dispose();
            cmdMessenger = new CmdMessenger(st);
            cmdMessenger.BoardType = BoardType.Bit16;
            AttachCommandCallBacks();
            cmdMessenger.StartListening();
            if (ConnectionStatusChanged != null) ConnectionStatusChanged(connected);
        }

        /// <summary>
        /// Disconnects from the communication port, sends an event for the connection status change
        /// </summary>
        private void Disconnect()
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
            connected = false;
            fixture = NO_FIXTURE;
            if (FixtureChanged != null) FixtureChanged(fixture);
            if (ConnectionStatusChanged != null) ConnectionStatusChanged(connected);
        }

        /// <summary>
        /// Attached the command callbacks from the serial communication 
        /// </summary>
        private void AttachCommandCallBacks()
        {
            cmdMessenger.Attach((int)Command.kModelChanged, OnModelChanged);
            cmdMessenger.Attach((int) Command.kPartStatus, OnPartStatusChanged);
        }

        /// <summary>
        /// Based on the fixture information try to send the command to the Electrox Machine to select the program that matches the fixture on place
        /// </summary>
        private void SendSelectionCommand()
        {
            if (fixture != NO_FIXTURE)
            {
                ElectroxProgramInfo program = ElectroxProgramManagment.GetInstance().GetProgramInfoFromId(fixture);
                if (program != null)
                {
                    bool found = false;
                    foreach (LaserProgram laserProg in laser.Programs)
                    {
                        if (program.Name == laserProg.Name)
                        {
                            found = true;                            
                        }
                    }
                    if (found)
                    {
                        if (partPresent)
                        {
                            laser.SelectProgram(program.Name);
                            if (FixtureSelectionChanged != null) FixtureSelectionChanged(program);
                        }
                        else
                        {
                            laser.Lock(true);
                            if (NoPartFound != null) NoPartFound();
                        }
                    }
                    else
                    {
                        laser.Lock(true);
                        if (NoProgramFound != null) NoProgramFound();
                    }
                }
                else
                {                    
                    laser.Lock(true);
                    if (InvalidFixture != null) InvalidFixture();
                }
            }
            else
            {
                laser.Lock(true);
                if (NoFixtureFound != null) NoFixtureFound();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the event for the model changed sent by the Fixture Manager via Serial Communication Port
        /// </summary>
        /// <param name="command"></param>
        private void OnModelChanged(ReceivedCommand command)
        {
            int tempFixture;
            tempFixture = int.Parse(command.ReadStringArg());
            if (tempFixture != fixture || fixture == NO_FIXTURE)
            {
                fixture = tempFixture;
                if (FixtureChanged != null) FixtureChanged(fixture);
            }
            SendSelectionCommand();
        }

        private void OnPartStatusChanged(ReceivedCommand command)
        {
            this.partPresent = command.ReadBoolArg();
            if (PartPresenceStatusChanged != null) PartPresenceStatusChanged(partPresent);
            SendSelectionCommand();
        }

        #endregion

    }
}