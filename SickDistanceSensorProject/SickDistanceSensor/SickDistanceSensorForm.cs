using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SickDistanceSensor
{
    public delegate void UpdateMeasurement(string value);

    public delegate void UpdateDisplayState(bool on);

    public partial class SickDistanceSensorForm : Form
    {
        /// <summary>
        /// Sick sensor object
        /// </summary>
        SickDistanceSensor sensor = new SickDistanceSensor();

        /// <summary>
        /// Timer to monitor Com port
        /// </summary>
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public SickDistanceSensorForm()
        {
            InitializeComponent();
            this.FormClosed += SickDistanceSensorForm_FormClosed;
            this.setFoundCommPort();
            this.sensor.MeasurementChanged += sensor_MeasurementChanged;
            this.sensor.DisplayStateUpdated += sensor_DisplayStateUpdated;
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick;
            this.timer.Start();
        }

        #region Sensor Event Handlers

        /// <summary>
        /// Handle sensor Display State Update to change the text of the button
        /// </summary>
        /// <param name="on"></param>
        void sensor_DisplayStateUpdated(bool on)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateDisplayState(updateDisplayState), on);
            }
        }

        /// <summary>
        /// Handle sensor measurement change
        /// </summary>
        /// <param name="value"></param>
        private void sensor_MeasurementChanged(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMeasurement(updateMeasurement), value);
            }
        }

        /// <summary>
        /// Update the measurement information on the UI
        /// </summary>
        /// <param name="value"></param>
        void updateMeasurement(string value)
        {
            this.informationField.Text = "Connected to Sick Sensor OD1-B015C05A14";
            this.measurementLabel.Text = value;
        }

        /// <summary>
        /// Updates the text of the display state button
        /// </summary>
        /// <param name="on"></param>
        private void updateDisplayState(bool on)
        {
            this.displayButton.Text = (on) ? "Display Off" : "Display On";
        }

        #endregion

        #region Timer Handler

        /// <summary>
        /// Timer tick handler, to monitor the arduino comm port availability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            setFoundCommPort();            
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Handle the form close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SickDistanceSensorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            sensor.Disconnect();
        }

        /// <summary>
        /// Handle connect button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (sensor.Connected == false)
            {
                sensor.Connect();
                this.connectButton.Text = "Connecting...";
                Thread.Sleep(1000);
                sensor.RequestDisplayState();
                this.connectButton.Text = "Disconnect";
                this.setZeroButton.Enabled = true;
                this.displayButton.Enabled = true;
                this.lockButton.Enabled = true;
                this.unlockButton.Enabled = true;
                sensor.StartMeasuring();
            }
            else
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Handle the set Zero button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setZeroButton_Click(object sender, EventArgs e)
        {
            this.sensor.SetZero();
        }

        /// <summary>
        /// Handle the display on/off button click, calls the sensor method setdisplay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayButton_Click(object sender, EventArgs e)
        {
            if (this.sensor.DisplayOn)
            {
                this.sensor.SetDisplay(false);
            }
            else
            {
                this.sensor.SetDisplay(true);
            }
        }

        /// <summary>
        /// Handle the lock button click event, calls the sensor setlock method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lockButton_Click(object sender, EventArgs e)
        {
            this.sensor.SetLock(true);
        }

        /// <summary>
        /// Handle the unlock button click event, calls the sensor setlock method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unlockButton_Click(object sender, EventArgs e)
        {
            this.sensor.SetLock(false);
        }

        #endregion

        private void Disconnect()
        {
            this.connectButton.Text = "Connect";
            this.measurementLabel.Text = "0.000";
            this.informationField.Text = "Disconnecting...";
            this.connectButton.Enabled = false;
            this.setZeroButton.Enabled = false;
            this.displayButton.Enabled = false;
            this.lockButton.Enabled = false;
            this.unlockButton.Enabled = false;
            sensor.Disconnect();
            setFoundCommPort();   
        }

        /// <summary>
        /// Gets the sensor comm port, if none set the not connected state of the form
        /// </summary>
        private void setFoundCommPort()
        {
            string commPort = sensor.CommPort.ToString();
            if (sensor.Connected == false)
            {
                if (commPort != String.Empty)
                {
                    this.informationField.Text = "Available Comm Port found at " + commPort;
                    this.connectButton.Enabled = true;                    
                }
                else
                {
                    this.informationField.Text = "No available Comm Port found";
                    this.connectButton.Enabled = false;
                    this.setZeroButton.Enabled = false;
                    this.displayButton.Enabled = false;
                    this.lockButton.Enabled = false;
                    this.unlockButton.Enabled = false;
                }
            }
            else
            {
                if (commPort == String.Empty)
                {
                    this.Disconnect();
                }
            }
        }
    }
}
