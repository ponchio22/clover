using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PipeClient
{
    
    class Client
    {
        public delegate void MessageReceivedHandler(string message);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
           String pipeName,
           uint dwDesiredAccess,
           uint dwShareMode,
           IntPtr lpSecurityAttributes,
           uint dwCreationDisposition,
           uint dwFlagsAndAttributes,
           IntPtr hTemplate);

        public const uint GENERIC_READ = (0x80000000);
        public const uint GENERIC_WRITE = (0x40000000);
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_FLAG_OVERLAPPED = (0x40000000);
        public const int BUFFER_SIZE = 4096;

        private FileStream stream;
        private SafeFileHandle handle;
        Thread readThread;        

        /// <summary>
        /// Reads data from the server
        /// </summary>
        public void Read()
        {
            this.stream = new FileStream(this.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);
            byte[] readBuffer = new byte[BUFFER_SIZE];
            ASCIIEncoding encoder = new ASCIIEncoding();
            while (true)
            {
                int bytesRead = 0;

                try
                {
                    bytesRead = this.stream.Read(readBuffer, 0, BUFFER_SIZE);
                }
                catch
                {
                    //read error occurred
                    break;
                }

                //server has disconnected
                if (bytesRead == 0)
                    break;

                //fire message received event
                if (this.MessageReceived != null)
                    this.MessageReceived(encoder.GetString(readBuffer, 0, bytesRead));
            }

            //clean up resource
            this.stream.Close();
            this.handle.Close();
        }        
                
        /// <summary>
        /// Name of the pipe to communicate
        /// </summary>
        private string pipeName;
        
        /// <summary>
        /// Deprecated: Indicates if its connected 
        /// </summary>
        private bool connected;

        /// <summary>
        /// Deprecated: Event triggered when a message is being received
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        public string PipeName
        {
            get { return this.pipeName; }
            set { this.pipeName = value; }
        }

        public bool Connected
        {
            get { return this.connected; }
        }

        /// <summary>
        /// Connects to the server
        /// </summary>
        public void Connect()
        {
            this.handle =
               CreateFile(
                  this.pipeName,
                  GENERIC_READ | GENERIC_WRITE,
                  0,
                  IntPtr.Zero,
                  OPEN_EXISTING,
                  FILE_FLAG_OVERLAPPED,
                  IntPtr.Zero);

            //could not create handle - server probably not running
            if (this.handle.IsInvalid)
                return;

            this.connected = true;

            //start listening for messages
            this.readThread = new Thread(Read);
            this.readThread.Name = "Client Read Thread for " + this.pipeName;
            this.readThread.Start();
            Thread.Sleep(50);
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            if (this.connected)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] messageBuffer = encoder.GetBytes(message);

                this.stream.Write(messageBuffer, 0, messageBuffer.Length);
                this.stream.Flush();
            }
        }

        public void Stop()
        {
            try
            {
                if (this.readThread != null)
                    this.readThread.Abort();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
