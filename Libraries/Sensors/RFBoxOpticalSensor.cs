using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valutech.IO;
using Valutech.Communication;
using System.Timers;
using System.Text.RegularExpressions;

namespace Valutech.Sensors
{
    public delegate void RFBoxOpticalSensorHandler(object sender, EventArgs args);

    public delegate void GeneralEventOpenedHandler(object sender,EventArgs args);

    public class RFBoxOpticalSensor
    {
        #region Events

        public event GeneralEventOpenedHandler ConnStateChanged;

        public event GeneralEventOpenedHandler StateChanged;

        #endregion

        #region Enum

        public enum CONN_STATE
        {
            CONNECTED,
            DISCONNECTED,
            CONNECTING
        }

        public enum STATE
        {
            OPEN,
            CLOSED
        }

        #endregion

        #region Variable Declaration

        public CONN_STATE connState = CONN_STATE.DISCONNECTED;

        public STATE state = STATE.CLOSED;
             
        private static RFBoxOpticalSensor instance;

        private SerialCommunication serial;

        private System.Timers.Timer timer = new System.Timers.Timer(50);

        private System.Timers.Timer disconnectedTimer = new System.Timers.Timer(1000);

        private System.Timers.Timer connectingTimer = new System.Timers.Timer(5000);

        private long elapsedTime = 0;
        
        #endregion

        private RFBoxOpticalSensor()
        {
            this.serial = new SerialCommunication();
            this.serial.END_OF_LINE = (char) 13;
            this.serial.PortName = "COM15";
            serial.PortConnected += new SerialDataPortEventHandler(serial_PortConnected);
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            disconnectedTimer.Elapsed += new ElapsedEventHandler(disconnectedTimer_Elapsed);
            connectingTimer.Elapsed += new ElapsedEventHandler(connectingTimer_Elapsed);
            disconnectedTimer.Start();            
        }

        void connectingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            disconnectedTimer.Stop();
            this.Connect();
        }

        void disconnectedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            disconnectedTimer.Stop();
            Connect();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            elapsedTime += (long) timer.Interval;
            if (elapsedTime > 500)
            {
                connState = CONN_STATE.DISCONNECTED;
                this.ConnStateChanged(this, EventArgs.Empty);
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
        }

        public void Connect()
        {
            this.connectingTimer.Start();
            this.connState = CONN_STATE.CONNECTING;
            this.ConnStateChanged(this, EventArgs.Empty);

            if (!serial.Open())
            {
                this.connState = CONN_STATE.DISCONNECTED;
                this.ConnStateChanged(this, EventArgs.Empty);
                disconnectedTimer.Start();
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
                this.ConnStateChanged(this, EventArgs.Empty);
            }
        }

        void serial_DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            timer.Stop();
            elapsedTime = 0;

            Regex regex = new Regex("111");
            if (!regex.IsMatch(args.Text))
            {
                if (state != STATE.CLOSED)
                {
                    state = STATE.CLOSED;
                    if (StateChanged != null) StateChanged(this, EventArgs.Empty);
                }                
            }
            else
            {
                if (state != STATE.OPEN)
                {
                    state = STATE.OPEN;
                    if (StateChanged != null) StateChanged(this, EventArgs.Empty);
                }   
            }
            timer.Start();
        }

        void serial_PortConnected(object sener, SerialDataPortEventArgs args)
        {
            connectingTimer.Stop();
            disconnectedTimer.Stop();
            connState = CONN_STATE.CONNECTED;
            this.ConnStateChanged(this, EventArgs.Empty);
            timer.Start();
        }

        public static RFBoxOpticalSensor GetInstance()
        {
            if (instance == null) instance = new RFBoxOpticalSensor();
            return instance;
        }
    }
}
