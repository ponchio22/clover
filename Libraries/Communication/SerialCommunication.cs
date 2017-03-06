using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using Valutech.IO;

namespace Valutech.Communication
{
    /// <summary>
    /// Event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void SerialDataSentEventHandler(object sender, SerialDataSentEventArgs args);

    /// <summary>
    /// Event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void SerialDataReceivedEventHandler(object sender, Valutech.IO.SerialDataReceivedEventArgs args);

    /// <summary>
    /// Event handler
    /// </summary>
    /// <param name="sener"></param>
    /// <param name="args"></param>
    public delegate void SerialDataPortEventHandler(object sener, SerialDataPortEventArgs args);

    /// <summary>
    /// Serial Communication class
    /// </summary>
    class SerialCommunication : SerialPort
    {

        /// <summary>
        /// Constant used to define the end of a line in the serial messages
        /// </summary>
        public char END_OF_LINE = ';';

        /// <summary>
        /// Instance of the class, used for singleton
        /// </summary>
        private static SerialCommunication instance;

        /// <summary>
        /// Event triggered every time some data has been sent by the Serial communication object
        /// </summary>
        public event SerialDataSentEventHandler DataSent;

        /// <summary>
        /// Event triggered every time some data has been received by the Serial communication object
        /// </summary>
        public new event SerialDataReceivedEventHandler DataReceived;

        /// <summary>
        /// Event triggered every time the port gets connected
        /// </summary>
        public event SerialDataPortEventHandler PortConnected;

        /// <summary>
        /// Buffer data
        /// </summary>
        private string bufferData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ComPort">Port to use in the class</param>
        public SerialCommunication()
            : base()
        {
            this.BaudRate = 9600;
            this.Parity = Parity.None;
            this.DataBits = 8;
            this.StopBits = StopBits.One;
            base.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serial_DataReceived);            
        }

        /// <summary>
        /// Get the instance of the class if it exists
        /// </summary>
        /// <returns></returns>
        public static SerialCommunication GetInstance()
        {
            if (SerialCommunication.instance == null) SerialCommunication.instance = new SerialCommunication();
            return SerialCommunication.instance;
        }

        /// <summary>
        /// Trigger the event Data sent
        /// </summary>
        /// <param name="args"></param>
        public void onDataSent(SerialDataSentEventArgs args)
        {
            if (DataSent != null) DataSent(this, args);
        }

        /// <summary>
        /// Trigger the event Data Received
        /// </summary>
        /// <param name="args"></param>
        public void onDataReceived(Valutech.IO.SerialDataReceivedEventArgs args)
        {
            if(DataReceived!= null) DataReceived(this,args);
        }

        /// <summary>
        /// Trigger the event Port Connected
        /// </summary>
        /// <param name="args"></param>
        public void onPortConnected(SerialDataPortEventArgs args)
        {
            if (PortConnected != null) PortConnected(this, args);
        }

        /// <summary>
        /// Take action based on the serial data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string indata = ReadExisting();
            bufferData += indata;
            if (bufferData.IndexOf(END_OF_LINE) >= 0)
            {
                string[] messages = bufferData.Split(END_OF_LINE);
                string message;
                bufferData = (messages.Length > 1) ? messages[messages.Length - 1] : String.Empty ;
                for(int i = 0;i< messages.Length-1 ;i++) 
                {
                    message = messages[i];
                    Valutech.IO.SerialDataReceivedEventArgs args = new Valutech.IO.SerialDataReceivedEventArgs();
                    args.Text = message;
                    onDataReceived(args);
                    //Debug.Print("Received: " + message);
                }
            };
        }

        /// <summary>
        /// Open seial port connection
        /// </summary>
        public new bool Open()
        {
            try
            {
                base.Open();
                SerialDataPortEventArgs args = new SerialDataPortEventArgs();
                args.PortName = this.PortName;
                onPortConnected(args);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Override the write method of the serial port object, to add the new data sent event
        /// </summary>
        /// <param name="text"></param>
        public new void Write(string text)
        {
            base.Write(text);
            SerialDataSentEventArgs args = new SerialDataSentEventArgs();
            args.Text = text;
            onDataSent(args);
        }

        /// <summary>
        /// Override the write line method of the serial port object, to add the new data sent event
        /// </summary>
        /// <param name="text"></param>
        public new void WriteLine(string text)
        {
            base.WriteLine(text);
            SerialDataSentEventArgs args = new SerialDataSentEventArgs();
            args.Text = text;
            onDataSent(args);
        }

        public void Send(string text)
        {
            base.Write(text + END_OF_LINE);
            SerialDataSentEventArgs args = new SerialDataSentEventArgs();
            args.Text = text + END_OF_LINE;
            onDataSent(args);
        }

    }
}