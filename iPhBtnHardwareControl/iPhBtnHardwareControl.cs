using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valutech.IO;
using Valutech.Communication;
using System.Timers;
using System.Text.RegularExpressions;
using AutoFindComLib;

namespace Valutech.Sensors
{
    public delegate void iPhBtnHardwareControlConnectionChangeEventHandler(iPhBtnHardwareControl.CONN_STATE state);    

    public delegate void PhonePowerStateChangedEventHandler(iPhBtnHardwareControl.POSITION position);

    public delegate void TestChangeEventHandler();

    public class iPhBtnHardwareControl
    {
        #region Events

        public event iPhBtnHardwareControlConnectionChangeEventHandler ConnectionStateChanged;
        
        public event PhonePowerStateChangedEventHandler PhoneTurnedOn;

        public event PhonePowerStateChangedEventHandler PhoneTurnedOff;

        public event TestChangeEventHandler TestStarted;

        public event TestChangeEventHandler TestFinished;

        #endregion

        #region Enum

        public enum CONN_STATE
        {
            CONNECTED,
            DISCONNECTED
        }

        public enum POSITION
        {
            LEFT,
            CENTER,
            RIGHT
        }

        //Incoming messages
        private string CONN_ALIVE = "LIVE";        
        private string TEST_STARTED = "TS";
        private string TEST_FINISHED = "TF";
        private string C1_ON = "C1S";
        private string C1_OFF = "C1F";
        private string C2_ON = "C2S";
        private string C2_OFF = "C2F";
        private string C3_ON = "C3S";
        private string C3_OFF = "C3F";

        //Outgoing messages
        private string START_TEST = "ST";
        private string C1_CLICK = "C1CLICK";
        private string C2_CLICK = "C2CLICK";
        private string C3_CLICK = "C3CLICK";

        #endregion

        #region Variable Declaration

        private CONN_STATE connState = CONN_STATE.DISCONNECTED;

        private static iPhBtnHardwareControl instance;

        private SerialCommunication serial;

        private System.Timers.Timer timer = new System.Timers.Timer(50);

        private System.Timers.Timer disconnectedTimer = new System.Timers.Timer(5000);

        private System.Timers.Timer connectingTimer = new System.Timers.Timer(5000);

        private long elapsedTime = 0;

        #endregion

        private iPhBtnHardwareControl()
        {
            this.serial = new SerialCommunication();
            AutoFindCom autofindComm = new AutoFindCom();
            this.serial.PortName = autofindComm.GetCommPort();
            serial.PortConnected += new SerialDataPortEventHandler(serial_PortConnected);
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            disconnectedTimer.Elapsed += new ElapsedEventHandler(disconnectedTimer_Elapsed);
            connectingTimer.Elapsed += new ElapsedEventHandler(connectingTimer_Elapsed);
            disconnectedTimer.Start();
        }

        private void connectingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            disconnectedTimer.Stop();
            this.Connect();
        }

        private void disconnectedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            disconnectedTimer.Stop();
            Connect();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            elapsedTime += (long)timer.Interval;
            if (elapsedTime > 500)
            {
                if (this.connState != CONN_STATE.DISCONNECTED)
                {
                    this.connState = CONN_STATE.DISCONNECTED;
                    this.ConnectionStateChanged(connState);
                    disconnectedTimer.Start();
                    timer.Stop();
                    try
                    {
                        serial.Close();
                    }
                    finally
                    {

                    }
                }
                this.connState = CONN_STATE.DISCONNECTED;
            }           
        }

        public void Connect()
        {
            this.connectingTimer.Start();
            if (!serial.Open())
            {
                if (this.connState != CONN_STATE.DISCONNECTED)
                {
                    this.connState = CONN_STATE.DISCONNECTED;
                    this.ConnectionStateChanged(connState);
                }
                this.connState = CONN_STATE.DISCONNECTED;
                disconnectedTimer.Start();
            }
            else
            {
                this.connState = CONN_STATE.CONNECTED;                
            }
        }

        public void Disconnect()
        {
            try
            {
                this.serial.Close();
            }
            finally
            {
                this.connState = CONN_STATE.DISCONNECTED;
                this.ConnectionStateChanged(connState);
            }
        }

        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            timer.Stop();
            if (args.Text == CONN_ALIVE)
            {
                elapsedTime = 0;
            }
            if (args.Text == C1_ON)
            {
                if (PhoneTurnedOn != null) PhoneTurnedOn(POSITION.LEFT);
            }
            if (args.Text == C1_OFF)
            {
                if (PhoneTurnedOff != null) PhoneTurnedOff(POSITION.LEFT);
            }
            if (args.Text == C2_ON)
            {
                if (PhoneTurnedOn != null) PhoneTurnedOn(POSITION.CENTER);
            }
            if (args.Text == C2_OFF)
            {
                if (PhoneTurnedOff != null) PhoneTurnedOff(POSITION.CENTER);
            }
            if (args.Text == C3_ON)
            {
                if (PhoneTurnedOn != null) PhoneTurnedOn(POSITION.RIGHT);
            }
            if (args.Text == C3_OFF)
            {
                if (PhoneTurnedOff != null) PhoneTurnedOff(POSITION.RIGHT);
            }
            if (args.Text == TEST_STARTED)
            {
                if (TestStarted != null) TestStarted();
            }
            if (args.Text == TEST_FINISHED)
            {
                if (TestFinished != null) TestFinished();
            }
            timer.Start();
        }

        public void StartTest()
        {
            serial.Send(START_TEST);
        }

        public void ActivatePosition(POSITION position)
        {
            if (position == POSITION.LEFT)
            {
                serial.Send(C1_CLICK);
            }
            else if (position == POSITION.CENTER)
            {
                serial.Send(C2_CLICK);
            }
            else
            {
                serial.Send(C3_CLICK);
            }
        }

        private void serial_PortConnected(object sener, SerialDataPortEventArgs args)
        {
            connectingTimer.Stop();
            disconnectedTimer.Stop();
            connState = CONN_STATE.CONNECTED;
            this.ConnectionStateChanged(connState);
            timer.Start();
        }

        public static iPhBtnHardwareControl GetInstance()
        {
            if (instance == null) instance = new iPhBtnHardwareControl();
            return instance;
        }

        public CONN_STATE ConnectionState
        {
            get
            {
                return this.connState;
            }
        }

    }
}
