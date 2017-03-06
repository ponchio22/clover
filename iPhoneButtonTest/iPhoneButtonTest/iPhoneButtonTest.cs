using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Valutech.Sensors;
using AutoFindComLib;

namespace iPhoneButtonTest
{
    public partial class iPhoneButtonTest : Form
    {

        public delegate void Invoker(iPhBtnHardwareControl.CONN_STATE state);

        private iPhBtnHardwareControl control = iPhBtnHardwareControl.GetInstance();

        private AutoFindCom autofindComm = new AutoFindCom();            
        
        public iPhoneButtonTest()
        {
            control.ConnectionStateChanged += control_ConnectionStateChanged;
            InitializeComponent();
            SetConnectionStatusLabelText();            
        }

        private void SetConnectionStatusLabelText()
        {
            connectionStatusLabel.Text = (control.ConnectionState == iPhBtnHardwareControl.CONN_STATE.CONNECTED)? "Connected on " + autofindComm.GetCommPort():"Disconnected";
        }

        void control_ConnectionStateChanged(iPhBtnHardwareControl.CONN_STATE state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Invoker(handleStateChange), state);
            }
            else
            {
                handleStateChange(state);
            }            
        }

        private void handleStateChange(iPhBtnHardwareControl.CONN_STATE state)
        {
            startTestButton.Enabled = (state == iPhBtnHardwareControl.CONN_STATE.CONNECTED);
            button1.Enabled = (state == iPhBtnHardwareControl.CONN_STATE.CONNECTED);
            button2.Enabled = (state == iPhBtnHardwareControl.CONN_STATE.CONNECTED);
            button3.Enabled = (state == iPhBtnHardwareControl.CONN_STATE.CONNECTED);
            SetConnectionStatusLabelText();
        }

        private void startTestButton_Click(object sender, EventArgs e)
        {
            control.StartTest();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            control.ActivatePosition(iPhBtnHardwareControl.POSITION.LEFT);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            control.ActivatePosition(iPhBtnHardwareControl.POSITION.CENTER);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            control.ActivatePosition(iPhBtnHardwareControl.POSITION.RIGHT);
        }

        

    }
}
