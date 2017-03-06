using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Valutech.Station
{
    public delegate void StationSummaryPathChangedEventHandler(string path);

    public class StationSummary
    {
        private Thread summaryThread;

        private string path = String.Empty;

        private StationSummaryFile summaryData;

        public event StationSummaryPathChangedEventHandler StationSummaryPathChanged;

        public StationSummary()
        {
            summaryThread = new Thread(new ThreadStart(UploadInfo));
            summaryData = new StationSummaryFile();
        }

        public void Start()
        {
            summaryThread.Start();
        }

        public void UploadInfo()
        {
            while (true)
            {
                try
                {
                    summaryData.Save();
                }
                catch
                {

                }
                Thread.Sleep(60000);
            }
        }

        public string Path
        {
            set
            {
                if (this.path != value)
                {
                    this.path = value;
                    if (this.StationSummaryPathChanged != null) this.StationSummaryPathChanged(this.path);
                }
            }
        }

        public void Stop()
        {
            summaryThread.Abort();
        }
    }
}
