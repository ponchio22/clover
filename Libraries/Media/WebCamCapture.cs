using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Timers;
using System.Drawing;
using System.Drawing.Imaging;
using Valutech.Media;
using DirectShowLib;
using System.Windows.Forms;

namespace Valutech.Media
{
    public delegate void WebCamEventHandler(object sender,WebcamEventArgs args);

    public class WebCamCapture : ISampleGrabberCB, IDisposable
    {
        #region APIs
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        #endregion

        #region DirectShow Private Variables Declaration

        private IFilterGraph2 m_FilterGraph = null;
        private IAMVideoControl m_VidControl = null;
        private IPin m_pinStill = null;
        private ManualResetEvent m_PictureReady = null;
        private IntPtr m_ipBuffer = IntPtr.Zero;

        #endregion

        #region Private Variables Declaration

        private static WebCamCapture instance;
        private bool started = false;
        private int captureWidth = 320;
        private int captureHeight = 240;
        private int captureStride;
        private int framesPerSecond = 24;
        private System.Timers.Timer timer = new System.Timers.Timer(40);
        private bool m_WantOne = false;
        private PictureBox pictureBox = new PictureBox();
        #endregion       

        #region Events

        public event WebCamEventHandler ImageCaptured;

        #endregion

        #region Constructor and Get Instance Method

        /// <summary>
        /// Constructor
        /// </summary>
        private WebCamCapture()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        /// <summary>
        /// Destructor
        /// </summary> 
        ~WebCamCapture()
        {
            Dispose();
        }

        /// <summary>
        /// Create a new instance if it doesnt already exists
        /// </summary>
        /// <returns>Unique instance of the object</returns>
        public static WebCamCapture GetInstance()
        {
            if (instance == null) instance = new WebCamCapture();
            return instance;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Trigger and event with the current webcam image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            WebcamEventArgs args = new WebcamEventArgs();
            args.WebCamImage = (Image) GetCurrentFrame();
            onImageCaptured(args);
            timer.Enabled = true;
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Gets the list of video devices found
        /// </summary>
        /// <returns></returns>
        private DsDevice[] GetDevices() {
            DsDevice[] capDevices;
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            return capDevices;
        }

        /// <summary>
        /// Set the Framerate, and video size
        /// </summary>
        /// <param name="pStill"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <param name="iBPP"></param>
        private void SetConfigParms(IPin pStill, int iWidth, int iHeight, short iBPP)
        {
            int hr;
            AMMediaType media;
            VideoInfoHeader v;

            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;

            // Get the existing format block
            hr = videoStreamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // copy out the videoinfoheader
                v = new VideoInfoHeader();
                Marshal.PtrToStructure(media.formatPtr, v);

                // if overriding the width, set the width
                if (iWidth > 0)
                {
                    v.BmiHeader.Width = iWidth;
                }

                // if overriding the Height, set the Height
                if (iHeight > 0)
                {
                    v.BmiHeader.Height = iHeight;
                }

                // if overriding the bits per pixel
                if (iBPP > 0)
                {
                    v.BmiHeader.BitCount = iBPP;
                }

                // Copy the media structure back
                Marshal.StructureToPtr(v, media.formatPtr, false);

                // Set the new format
                hr = videoStreamConfig.SetFormat(media);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                DsUtils.FreeAMMediaType(media);
                media = null;
            }
        }

        /// <summary>
        /// Set the video window within the control specified by hControl
        /// </summary>
        /// <param name="hControl"></param>
        private void ConfigVideoWindow(Control hControl)
        {
            int hr;

            IVideoWindow ivw = m_FilterGraph as IVideoWindow;

            // Set the parent
            hr = ivw.put_Owner(hControl.Handle);
            DsError.ThrowExceptionForHR(hr);

            // Turn off captions, etc
            hr = ivw.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            DsError.ThrowExceptionForHR(hr);

            // Yes, make it visible
            hr = ivw.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);

            // Move to upper left corner
            Rectangle rc = hControl.ClientRectangle;
            hr = ivw.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Configure the sample grabber
        /// </summary>
        /// <param name="sampGrabber"></param>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            int hr;
            AMMediaType media = new AMMediaType();

            // Set the media type to Video/RBG24
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Shut down capture
        /// </summary>
        private void CloseInterfaces()
        {
            int hr;

            try
            {
                if (m_FilterGraph != null)
                {
                    IMediaControl mediaCtrl = m_FilterGraph as IMediaControl;

                    // Stop the graph
                    hr = mediaCtrl.Stop();
                }
            }
            catch
            {
            }

            if (m_FilterGraph != null)
            {
                Marshal.ReleaseComObject(m_FilterGraph);
                m_FilterGraph = null;
            }

            if (m_VidControl != null)
            {
                Marshal.ReleaseComObject(m_VidControl);
                m_VidControl = null;
            }

            if (m_pinStill != null)
            {
                Marshal.ReleaseComObject(m_pinStill);
                m_pinStill = null;
            }
        }

        /// <summary>
        /// Get the actual size info from the video and save it to local variables
        /// </summary>
        /// <param name="sampGrabber"></param>
        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();

            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            captureWidth = videoInfoHeader.BmiHeader.Width;
            captureHeight = videoInfoHeader.BmiHeader.Height;
            captureStride = captureWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// Send the event image captured
        /// </summary>
        /// <param name="args"></param>
        private void onImageCaptured(WebcamEventArgs args)
        {
            if (ImageCaptured != null) ImageCaptured(this, args);
        }

        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (m_WantOne)
            {
                m_WantOne = false;
                CopyMemory(m_ipBuffer, pBuffer, BufferLen);
                m_PictureReady.Set();
            }
            return 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start using the camera
        /// </summary>
        /// <returns>Indicate if the webcam was able to start</returns>
        public bool Start()
        {
            if (!started)
            {
                DsDevice[] devices = GetDevices();
                if (devices.Length > 0)
                {
                    DsDevice dev = devices[0];
                    
                    // Initialize camera
                    int hr;                    
                    IBaseFilter capFilter = null;
                    ISampleGrabber sampGrabber = null;
                    IPin pCaptureOut = null;
                    IPin pSampleIn = null;
                    IPin pRenderIn = null;

                    m_FilterGraph = new FilterGraph() as IFilterGraph2;
                    try
                    {
                        hr = m_FilterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
                        if (m_pinStill == null)
                        {
                            m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
                        }

                        // Still haven't found one.  Need to put a splitter in so we have
                        // one stream to capture the bitmap from, and one to display.  Ok, we
                        // don't *have* to do it that way, but we are going to anyway.
                        if (m_pinStill == null)
                        {
                            IPin pRaw = null;
                            IPin pSmart = null;

                            // There is no still pin
                            m_VidControl = null;

                            // Add a splitter
                            IBaseFilter iSmartTee = (IBaseFilter)new SmartTee();

                            try
                            {
                                hr = m_FilterGraph.AddFilter(iSmartTee, "SmartTee");
                                DsError.ThrowExceptionForHR(hr);

                                // Find the find the capture pin from the video device and the
                                // input pin for the splitter, and connnect them
                                pRaw = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                                pSmart = DsFindPin.ByDirection(iSmartTee, PinDirection.Input, 0);

                                hr = m_FilterGraph.Connect(pRaw, pSmart);
                                DsError.ThrowExceptionForHR(hr);

                                // Now set the capture and still pins (from the splitter)
                                m_pinStill = DsFindPin.ByName(iSmartTee, "Preview");
                                pCaptureOut = DsFindPin.ByName(iSmartTee, "Capture");

                                // If any of the default config items are set, perform the config
                                // on the actual video device (rather than the splitter)
                                if (captureHeight + captureWidth > 0)
                                {
                                    SetConfigParms(pRaw, captureWidth, captureHeight, 24);
                                }
                            }
                            finally
                            {
                                if (pRaw != null)
                                {
                                    Marshal.ReleaseComObject(pRaw);
                                }
                                if (pRaw != pSmart)
                                {
                                    Marshal.ReleaseComObject(pSmart);
                                }
                                if (pRaw != iSmartTee)
                                {
                                    Marshal.ReleaseComObject(iSmartTee);
                                }
                            }
                        }
                        else
                        {
                            // Get a control pointer (used in Click())
                            m_VidControl = capFilter as IAMVideoControl;

                            pCaptureOut = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);

                            // If any of the default config items are set
                            if (captureHeight + captureWidth> 0)
                            {
                                SetConfigParms(m_pinStill, captureWidth, captureHeight, 24);
                            }
                        }

                        // Get the SampleGrabber interface
                        sampGrabber = new SampleGrabber() as ISampleGrabber;

                        // Configure the sample grabber
                        IBaseFilter baseGrabFlt = sampGrabber as IBaseFilter;
                        ConfigureSampleGrabber(sampGrabber);

                        pSampleIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

                        // Get the default video renderer
                        IBaseFilter pRenderer = new VideoRendererDefault() as IBaseFilter;
                        hr = m_FilterGraph.AddFilter(pRenderer, "Renderer");
                        DsError.ThrowExceptionForHR(hr);

                        pRenderIn = DsFindPin.ByDirection(pRenderer, PinDirection.Input, 0);

                        // Add the sample grabber to the graph
                        hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                        DsError.ThrowExceptionForHR(hr);

                        if (m_VidControl == null)
                        {
                            // Connect the Still pin to the sample grabber
                            hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                            DsError.ThrowExceptionForHR(hr);

                            // Connect the capture pin to the renderer
                            hr = m_FilterGraph.Connect(pCaptureOut, pRenderIn);
                            DsError.ThrowExceptionForHR(hr);
                        }
                        else
                        {
                            // Connect the capture pin to the renderer
                            hr = m_FilterGraph.Connect(pCaptureOut, pRenderIn);
                            DsError.ThrowExceptionForHR(hr);

                            // Connect the Still pin to the sample grabber
                            hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                            DsError.ThrowExceptionForHR(hr);
                        }
                        SaveSizeInfo(sampGrabber);
                        ConfigVideoWindow(pictureBox);

                        IMediaControl mediaCtrl = m_FilterGraph as IMediaControl;
                        hr = mediaCtrl.Run();
                        DsError.ThrowExceptionForHR(hr);
                    }
                    finally
                    {
                        if (sampGrabber != null)
                        {
                            Marshal.ReleaseComObject(sampGrabber);
                            sampGrabber = null;
                        }
                        if (pCaptureOut != null)
                        {
                            Marshal.ReleaseComObject(pCaptureOut);
                            pCaptureOut = null;
                        }
                        if (pRenderIn != null)
                        {
                            Marshal.ReleaseComObject(pRenderIn);
                            pRenderIn = null;
                        }
                        if (pSampleIn != null)
                        {
                            Marshal.ReleaseComObject(pSampleIn);
                            pSampleIn = null;
                        }
                    }
                    m_PictureReady = new ManualResetEvent(false);
                    timer.Interval = (int)(1000 / framesPerSecond);
                    timer.Start();
                    return true;                    
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        public void Start(int frame)
        {
            this.Start();
        }

        /// <summary>
        /// Stop using the camera
        /// </summary>
        public void Stop()
        {
            started = false;
            timer.Stop();
            CloseInterfaces();
        }

        /// <summary>
        /// Get the image from the Still pin.  The returned image can turned into a bitmap with
        /// Bitmap b = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, m_ip);
        /// If the image is upside down, you can fix it with
        /// b.RotateFlip(RotateFlipType.RotateNoneFlipY);
        /// </summary>
        /// <returns>Returned pointer to be freed by caller with Marshal.FreeCoTaskMem</returns>
        public Bitmap GetCurrentFrame()
        {
            int hr;
            try
            {
                // get ready to wait for new image
                m_PictureReady.Reset();
                m_ipBuffer = Marshal.AllocCoTaskMem(Math.Abs(captureStride) * captureHeight);
            }
            catch
            {
            }
            try
            {
                m_WantOne = true;

                // If we are using a still pin, ask for a picture
                if (m_VidControl != null)
                {
                    // Tell the camera to send an image
                    hr = m_VidControl.SetMode(m_pinStill, VideoControlFlags.Trigger);
                    DsError.ThrowExceptionForHR( hr );
                }

                // Start waiting
                if ( ! m_PictureReady.WaitOne(9000, false) )
                {
                    throw new Exception("Timeout waiting to get picture");
                }
            }
            catch
            {
                Marshal.FreeCoTaskMem(m_ipBuffer);
                m_ipBuffer = IntPtr.Zero;                
            }
	
            // Got one
            Bitmap b = new Bitmap(captureWidth, captureHeight, captureStride, PixelFormat.Format24bppRgb, m_ipBuffer);
            return b;
        }

        /// <summary>
        /// Release everything. 
        /// </summary>
        public void Dispose()
        {
            CloseInterfaces();
            if (m_PictureReady != null)
            {
                m_PictureReady.Close();
            }
        }

        #endregion

        #region Getters and Setters

        /// <summary>
        /// output the started variable value
        /// </summary>
        public bool Started
        {
            get
            {
                return started;
            }
        }

        /// <summary>
        /// Set/Get the value of the capture width
        /// </summary>
        public int CaptureWidth
        {
            set
            {
                captureWidth = value;
            }
            get
            {
                return captureWidth;
            }
        }

        /// <summary>
        /// Set/Get the value of the capture height
        /// </summary>
        public int CaptureHeight
        {
            set
            {
                captureHeight = value;
            }
            get
            {
                return captureHeight;
            }
        }

        #endregion
    }
}
