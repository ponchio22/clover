using System;
using System.Collections.Generic;
using System.Text;

namespace Valutech.Media
{
    public class WebCam
    {
        private WebCamCapture webcam;

        private System.Windows.Forms.PictureBox _FrameImage;

        private int FrameNumber = 30;

        public void InitializeWebCam(ref System.Windows.Forms.PictureBox ImageControl)
        {
            webcam = WebCamCapture.GetInstance();
            /*
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;*/
            webcam.ImageCaptured += new WebCamEventHandler(webcam_ImageCaptured);
            _FrameImage = ImageControl;
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            if (_FrameImage.Image != null) _FrameImage.Image.Dispose();
            _FrameImage.Image = e.WebCamImage;
        }

        public void Start()
        {
            webcam.Start(0);
        }

        public void Stop()
        {
            webcam.Stop();
        }

        public void Continue()
        {            
            // resume the video capture from the stop
            webcam.Start(0);
        }

        public void ResolutionSetting()
        {
            //webcam.Config();
        }

        public void AdvanceSetting()
        {
            //webcam.Config2();
        }

    }
}
