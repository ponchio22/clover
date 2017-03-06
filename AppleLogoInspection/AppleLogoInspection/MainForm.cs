using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Controls;
using AForge.Video.DirectShow;
using AForge.Video.DirectShow.Internals;
using System.Threading;

namespace AppleLogoInspection
{
    public partial class MainForm : Form
    {
        private FilterInfoCollection videoDevices;

        public VideoCaptureDevice videoSource;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            //Fill the video devices in the combo
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0) throw new ApplicationException();
                foreach (FilterInfo device in videoDevices)
                camerasCombo.Items.Add(device.Name);
                camerasCombo.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                camerasCombo.Items.Add("No local capture devices");
                videoDevices = null;
            }            
        }

        private void startButton_Click(object sender, EventArgs e)
        {            
            videoPlayer.Stop();
            videoPlayer.WaitForStop();
            videoSource = new VideoCaptureDevice(videoDevices[camerasCombo.SelectedIndex].MonikerString);
            videoSource.VideoResolution = videoSource.VideoCapabilities[0];
            videoPlayer.VideoSource = videoSource;
            Thread.Sleep(500);
            videoPlayer.Start();
            Thread.Sleep(5000);            
            controlCameraSettings();
        }

        private void controlCameraSettings()
        {
            videoSource.SetCameraProperty(CameraControlProperty.Focus, (int) ((255 / 15) * 15), CameraControlFlags.Manual);
            videoSource.SetCameraProperty(CameraControlProperty.Zoom, 2, CameraControlFlags.Manual);
            videoSource.SetCameraProperty(CameraControlProperty.Pan, 1, CameraControlFlags.Manual);
            videoSource.SetCameraProperty(CameraControlProperty.Tilt, -2, CameraControlFlags.Manual);

            
            object o;
            Guid IID_IBaseFilter = new Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
            
            IAMVideoProcAmp vpa = (IAMVideoProcAmp) o;
            int pMin, pMax, pSteppingDelta, pDefault;
            VideoProcAmpFlags pFlags;   
            vpa.GetRange(
            VideoProcAmpProperty.Brightness,
            out pMin,
            out pMax,
            out pSteppingDelta,
            out pDefault,
            out pFlags);
            
            vpa.Set(VideoProcAmpProperty.Gain, 20, pFlags);
            vpa.Set(VideoProcAmpProperty.Contrast, 70, pFlags);
        }
    }
}
