using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using Valutech.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Valutech.IO.SetupClasses
{
    #region Shortcut Class
    public class Shortcut
    {

        private static Type m_type = Type.GetTypeFromProgID("WScript.Shell");
        private static object m_shell = Activator.CreateInstance(m_type);

        [ComImport, TypeLibType((short)0x1040), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
        private interface IWshShortcut
        {
            [DispId(0)]
            string FullName { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0)] get; }
            [DispId(0x3e8)]
            string Arguments { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3e8)] set; }
            [DispId(0x3e9)]
            string Description { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3e9)] set; }
            [DispId(0x3ea)]
            string Hotkey { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ea)] set; }
            [DispId(0x3eb)]
            string IconLocation { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3eb)] set; }
            [DispId(0x3ec)]
            string RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ec)] set; }
            [DispId(0x3ed)]
            string TargetPath { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ed)] set; }
            [DispId(0x3ee)]
            int WindowStyle { [DispId(0x3ee)] get; [param: In] [DispId(0x3ee)] set; }
            [DispId(0x3ef)]
            string WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ef)] set; }
            [TypeLibFunc((short)0x40), DispId(0x7d0)]
            void Load([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
            [DispId(0x7d1)]
            void Save();
        }

        public static void Create(string fileName, string targetPath, string arguments, string workingDirectory, string description, string hotkey, string iconPath)
        {
            IWshShortcut shortcut = (IWshShortcut)m_type.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, m_shell, new object[] { fileName });
            shortcut.Description = description;
            shortcut.Hotkey = hotkey;
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = workingDirectory;
            shortcut.Arguments = arguments;
            if (!string.IsNullOrEmpty(iconPath))
                shortcut.IconLocation = iconPath;
            shortcut.Save();
        }
    }
    #endregion

    public class Setup
    {
        #region Event Delegates
        public delegate void SetupStateChanged(STATE state);
        public delegate void SetupProgressChanged(int progress);
        public delegate void SetupCurrentActivityChanged(string text);
        #endregion

        #region Private Variables
        private STATE currentState = STATE.IDLE;
        private int CurrentStepNumber = 0;
        private int CurrentProgress = 0;
        private int TotalSteps = 10;
        private int ProgressItemCount = 0;
        private List<ServiceToStop> servicesToStop = new List<ServiceToStop>();
        private ArrayList processesToStop = new ArrayList();
        private ArrayList filesToDelete = new ArrayList();
        private List<FileToCopy> filesToCopy = new List<FileToCopy>();
        private List<ApplicationToRun> appsToExecute = new List<ApplicationToRun>();
        private ArrayList servicesToRun = new ArrayList();
        private ArrayList cleanUpFiles = new ArrayList();
        private List<RegistryEntry> registryEntriesToAdd = new List<RegistryEntry>();
        private List<ShortcutEntry> shortcutsToCreate = new List<ShortcutEntry>();

        #endregion

        #region Structures
        /// <summary>
        /// Structure of a File to Copy
        /// </summary>
        private struct FileToCopy
        {
            public string source, destiny;
            public FileToCopy(string source, string destiny)
            {
                this.source = source;
                this.destiny = destiny;
            }
        }

        /// <summary>
        /// Structure of an Application to run
        /// </summary>
        private struct ApplicationToRun
        {
            public string applicationPath;
            public bool waitForApplicationToEnd;
            public bool visible;
            public string arguments;
            public ApplicationToRun(string applicationPath, bool waitForApplicationToEnd)
            {
                this.applicationPath = applicationPath;
                this.waitForApplicationToEnd = waitForApplicationToEnd;
                this.visible = false;
                this.arguments = String.Empty;
            }
            public ApplicationToRun(string applicationPath, bool waitForApplicationToEnd, bool visible)
            {
                this.applicationPath = applicationPath;
                this.waitForApplicationToEnd = waitForApplicationToEnd;
                this.visible = visible;
                this.arguments = string.Empty;
            }
            public ApplicationToRun(string applicationPath, bool waitForApplicationToEnd, bool visible, string arguments)
            {
                this.applicationPath = applicationPath;
                this.waitForApplicationToEnd = waitForApplicationToEnd;
                this.visible = visible;
                this.arguments = arguments;
            }
        }

        /// <summary>
        /// Structure of a service to stop
        /// </summary>
        private struct ServiceToStop
        {
            public string serviceName;
            public bool uninstall;
            public ServiceToStop(string serviceName, bool uninstall)
            {
                this.serviceName = serviceName;
                this.uninstall = uninstall;
            }
        }

        private struct RegistryEntry
        {
            public string path;
            public string key;
            public string stringvalue;
            public int intvalue;
            public Type type;
            public enum Type
            {
                STRING,
                INT
            }
            public RegistryEntry(string path, string key, string value)
            {
                this.path = path;
                this.key = key;
                this.stringvalue = value;
                this.intvalue = -1;
                this.type = Type.STRING;
            }
            public RegistryEntry(string path, string key, int value)
            {
                this.path = path;
                this.key = key;
                this.intvalue = value;
                this.stringvalue = String.Empty;
                this.type = Type.INT;
            }
            public object value
            {
                get
                {
                    if (type == Type.INT)
                        return intvalue;
                    else
                        return stringvalue;
                }
            }
        }

        private struct ShortcutEntry
        {
            public string path;
            public string name;
            public ShortcutEntry(string path, string name)
            {
                this.path = path;
                this.name = name;
            }
        }
        #endregion

        #region Enums
        public enum STATE
        {
            IDLE,
            STOPPING_SERVICES,
            STOPPING_PROCESSES,
            DELETING_FILES,
            COPYING_FILES,
            EXECUTING_APPS,
            RUNNING_SERVICES,
            CLEANING_INSTALLATION_FILES,
            ADDING_VALUES_TO_REGISTRY,
            CREATING_SHORTCUTS
        }
        #endregion

        #region Events
        public event SetupStateChanged StateChanged;
        public event SetupProgressChanged ProgressChanged;
        public event SetupCurrentActivityChanged CurrentActivityChanged;
        #endregion

        #region Exceptions
        [Serializable]
        public class WrongStateException : Exception
        {
            public WrongStateException()
                : base() { }

            public WrongStateException(string message)
                : base(message) { }

            public WrongStateException(string message, Exception innerException)
                : base(message, innerException) { }
        }
        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        public Setup()
        {
        }

        /// <summary>
        /// Adds a service to stop and uninstall during the setup
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        public void AddServiceToStop(string serviceName)
        {
            servicesToStop.Add(new ServiceToStop(serviceName, true));
        }

        /// <summary>
        /// Adds a service to stop during the setup
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="uninstall">Wheter you also want to uninstall or not</param>
        public void AddServiceToStop(string serviceName, bool uninstall)
        {
            servicesToStop.Add(new ServiceToStop(serviceName, uninstall));
        }

        /// <summary>
        /// Adds a process to stop
        /// </summary>
        /// <param name="processName">Name of the process</param>
        public void AddProcessToStop(string processName)
        {
            processesToStop.Add(processName);
        }

        /// <summary>
        /// Adds a path of a file or directory to delete
        /// </summary>
        /// <param name="path">Path of the file or directory to delete</param>
        public void AddPathToDelete(string path)
        {
            filesToDelete.Add(path);
        }

        /// <summary>
        /// Add a path to a file or directory to copy
        /// </summary>
        /// <param name="source">Path to the source file or directory</param>
        /// <param name="destiny">Path to the destiny file or directory</param>
        public void AddPathToCopy(string source, string destiny)
        {
            filesToCopy.Add(new FileToCopy(source, destiny));
        }

        /// <summary>
        /// Adds an application to be executed during setup
        /// </summary>
        /// <param name="applicationPath">Path to the application</param>
        public void AddApplicationToExecute(string applicationPath, bool waitForAppToEnd)
        {
            appsToExecute.Add(new ApplicationToRun(applicationPath, waitForAppToEnd));
        }

        /// <summary>
        /// Adds an application to be executed during setup
        /// </summary>
        /// <param name="applicationPath">Path to the application</param>
        public void AddApplicationToExecute(string applicationPath, bool waitForAppToEnd, bool visible)
        {
            appsToExecute.Add(new ApplicationToRun(applicationPath, waitForAppToEnd, visible));
        }

        /// <summary>
        /// Adds an application to be executed during setup
        /// </summary>
        /// <param name="applicationPath">Path to the application</param>
        public void AddApplicationToExecute(string applicationPath, bool waitForAppToEnd, bool visible, string arguments)
        {
            appsToExecute.Add(new ApplicationToRun(applicationPath, waitForAppToEnd, visible, arguments));
        }

        /// <summary>
        /// Adds a service to run during setup
        /// </summary>
        /// <param name="serviceName"></param>
        public void AddServiceToRun(string serviceName)
        {
            servicesToRun.Add(serviceName);
        }

        /// <summary>
        /// Add a clean up files 
        /// </summary>
        /// <param name="path"></param>
        public void AddCleanUpFiles(string path)
        {
            cleanUpFiles.Add(path);
        }

        /// <summary>
        /// Add a registry entries
        /// </summary>
        /// <param name="path"></param>
        public void AddRegistryEntries(string path, string key, string value)
        {
            registryEntriesToAdd.Add(new RegistryEntry(path, key, value));
        }

        /// <summary>
        /// Add a registry entries
        /// </summary>
        /// <param name="path"></param>
        public void AddRegistryEntries(string path, string key, int value)
        {
            registryEntriesToAdd.Add(new RegistryEntry(path, key, value));
        }

        /// <summary>
        /// Create a shortcut
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public void AddShortcutEntries(string path, string name)
        {
            shortcutsToCreate.Add(new ShortcutEntry(path, name));
        }

        /// <summary>
        /// Execute the setup actions
        /// </summary>
        public void Run()
        {
            if (currentState == STATE.IDLE)
            {

                CurrentState = STATE.STOPPING_SERVICES;
                CurrentStepNumber = 1;
                Console.WriteLine("{0}", "Stopping Services");
                this.StopServices(servicesToStop);

                CurrentState = STATE.STOPPING_PROCESSES;
                CurrentStepNumber = 2;
                Console.WriteLine("{0}", "Stopping processes");
                this.StopProcesses(processesToStop);

                Thread.Sleep(10000);
                try
                {
                    CurrentState = STATE.DELETING_FILES;
                    CurrentStepNumber = 3;
                    Console.WriteLine("{0}", "Deleting Files");
                    this.DeleteFiles(filesToDelete);
                }
                catch (Exception)
                {
                }

                CurrentState = STATE.ADDING_VALUES_TO_REGISTRY;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Adding values to registry");
                this.AddValuesToRegistry(registryEntriesToAdd);

                CurrentState = STATE.COPYING_FILES;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Copying Files");
                this.CopyFiles(filesToCopy);

                CurrentState = STATE.EXECUTING_APPS;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Executing Applications");
                this.ExecuteFiles(appsToExecute);

                CurrentState = STATE.RUNNING_SERVICES;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Running Services");
                this.RunServices(servicesToRun);

                CurrentState = STATE.CLEANING_INSTALLATION_FILES;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Deleting Files");
                this.DeleteFiles(cleanUpFiles);

                CurrentState = STATE.CREATING_SHORTCUTS;
                CurrentStepNumber++;
                Console.WriteLine("{0}", "Creating Shortcuts");
                this.CreateShortcuts(shortcutsToCreate);

                CurrentState = STATE.IDLE;
                CurrentStepNumber++;
                this.calculateProgress(CurrentStepNumber, 1, 1);
            }
            else
            {
                throw new WrongStateException(String.Format("Setup only can run if the status is {0}, CurrentState: {1}", STATE.IDLE.ToString(), CurrentState));
            }
        }

        private void onCurrentActivityChanged(string text)
        {
            if (CurrentActivityChanged != null) CurrentActivityChanged(text);
        }
        /// <summary>
        /// Stop the required services
        /// </summary>
        /// <param name="servicesToStop">List of services to stop</param>
        private void StopServices(List<ServiceToStop> servicesToStop)
        {
            ProgressItemCount = 0;
            foreach (ServiceToStop serviceItem in servicesToStop)
            {
                try
                {
                    string service = serviceItem.serviceName;
                    bool uninstall = serviceItem.uninstall;
                    this.onCurrentActivityChanged("Deteniendo " + service);
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(service))
                    {
                        ServiceController sc = new ServiceController(service);
                        do
                        {
                            if (sc.Status == ServiceControllerStatus.Running)
                            {
                                ProgressItemCount++;
                                this.calculateProgress(CurrentStepNumber, servicesToStop.Count, ProgressItemCount);
                                sc.Stop();
                            }
                            sc.Refresh();
                            if (sc.Status == ServiceControllerStatus.Stopped)
                            {
                                if (uninstall)
                                {
                                    try
                                    {
                                        ServiceTools.ServiceInstaller.Uninstall(service);
                                    }
                                    finally { }
                                }
                            }
                        } while (sc.Status != ServiceControllerStatus.Stopped);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Stop the required processes
        /// </summary>
        /// <param name="processesToStop">List of processes to stop</param>
        private void StopProcesses(ArrayList processesToStop)
        {
            ProgressItemCount = 0;
            foreach (string process in processesToStop)
            {
                this.onCurrentActivityChanged("Deteniendo " + process);
                ProgressItemCount++;
                this.calculateProgress(CurrentStepNumber, processesToStop.Count, ProgressItemCount);
                foreach (Process myProc in Process.GetProcesses())
                {
                    try
                    {
                        if (myProc.ProcessName == process) myProc.Kill();
                        Console.WriteLine("{0} {1}", process, "closed");
                    }
                    catch
                    {
                    }
                }
            }
            Thread.Sleep(1000);
            AppManipulation.RefreshTrayArea();
        }

        void calculateProgress(int currentStep, int total, int count)
        {
            Console.WriteLine("Total Steps: {0} CurrentStep: {1} Total: {2} count: {3}", TotalSteps, currentStep, total, count);
            CurrentProgress = ((100 / TotalSteps) * (currentStep - 1)) + (((100 / TotalSteps) / total) * count);
            if (ProgressChanged != null) ProgressChanged(CurrentProgress);
        }



        #region DeleteFiles Methods
        /// <summary>
        /// Delete the required files or directories
        /// </summary>
        /// <param name="filesToDelete"></param>
        private void DeleteFiles(ArrayList filesToDelete)
        {
            int count = 0;
            foreach (string filePath in filesToDelete)
            {
                if (System.IO.File.Exists(filePath) || Directory.Exists(filePath))
                {
                    FileAttributes attr = System.IO.File.GetAttributes(filePath);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        DeleteFolder(filePath);
                    }
                    else
                    {
                        count++;
                        this.calculateProgress(CurrentStepNumber, filesToDelete.Count, count);
                        DeleteFile(filePath);
                    }
                }
            }
        }

        /// <summary>
        /// Delete a specific file
        /// </summary>
        /// <param name="filePath"></param>
        private void DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine("{0} {1}", filePath, "deleted");
                }
            }
            catch
            {
            }
            finally { }
        }

        /// <summary>
        /// Deletes a folder recursively
        /// </summary>
        /// <param name="?"></param>
        private void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                //Get the files inside the folder
                string[] files = Directory.GetFiles(folderPath);
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        DeleteFile(file);
                    }
                }

                //Get the folders inside
                string[] directories = Directory.GetDirectories(folderPath);
                if (directories.Length > 0)
                {
                    foreach (string directory in directories)
                    {
                        DeleteFolder(directory);
                    }
                }

                //Delete folder
                try
                {
                    Directory.Delete(folderPath);
                    Console.WriteLine("{0} {1}", folderPath, "deleted");
                }
                finally { }
            }
        }
        #endregion

        #region CopyFiles Methods
        /// <summary>
        /// Copy the required files or directories
        /// </summary>
        /// <param name="filesToCopy"></param>
        private void CopyFiles(List<FileToCopy> filesToCopy)
        {
            int count = 0;
            foreach (FileToCopy copyFileItem in filesToCopy)
            {
                string sourcePath = copyFileItem.source;
                string destinyPath = copyFileItem.destiny;

                this.onCurrentActivityChanged("Copiando " + destinyPath);

                if (System.IO.File.Exists(sourcePath) || Directory.Exists(sourcePath))
                {
                    FileAttributes attr = System.IO.File.GetAttributes(sourcePath);
                    if (attr == FileAttributes.Directory)
                    {
                        CopyFolder(copyFileItem);
                    }
                    else
                    {
                        CopyFile(copyFileItem);
                        count++;
                        this.calculateProgress(CurrentStepNumber, filesToCopy.Count, count);
                    }
                }
            }
        }

        /// <summary>
        /// Copies a specific file
        /// </summary>
        /// <param name="filePath"></param>
        private void CopyFile(FileToCopy copyFileItem)
        {
            string sourcePath = copyFileItem.source;
            string destinyPath = copyFileItem.destiny;
            try
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinyPath));
                    if (System.IO.File.Exists(destinyPath))
                    {
                        string suffix = ".copy";
                        System.IO.File.Copy(sourcePath, destinyPath + suffix, true);
                        System.IO.File.Delete(destinyPath);
                        System.IO.File.Copy(destinyPath + suffix, destinyPath, true);
                        System.IO.File.Delete(destinyPath + suffix);
                        Console.WriteLine("{0} {1}", destinyPath, " file overwritten");
                    }
                    else
                    {
                        System.IO.File.Copy(sourcePath, destinyPath);
                        Console.WriteLine("{0} {1}", destinyPath, " file copied");
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Copies a folder recursively
        /// </summary>
        /// <param name="?"></param>
        private void CopyFolder(FileToCopy copyFileItem)
        {
            string sourcePath = copyFileItem.source;
            string destinyPath = copyFileItem.destiny;
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destinyPath))
                {
                    Directory.CreateDirectory(destinyPath);
                }

                //Get the files inside the folder
                string[] files = Directory.GetFiles(sourcePath);
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        CopyFile(new FileToCopy(copyFileItem.source + @"\" + Path.GetFileName(file), copyFileItem.destiny + @"\" + Path.GetFileName(file)));
                    }
                }

                //Get the folders inside
                string[] directories = Directory.GetDirectories(sourcePath);
                if (directories.Length > 0)
                {
                    foreach (string directory in directories)
                    {
                        string origin = Path.Combine(copyFileItem.source, directory.Replace(Path.GetDirectoryName(directory) + "\\", string.Empty));
                        string destiny = Path.Combine(copyFileItem.destiny, directory.Replace(Path.GetDirectoryName(directory) + "\\", string.Empty));
                        CopyFolder(new FileToCopy(origin, destiny));
                    }
                }
            }
        }
        #endregion

        private void CreateShortcuts(List<ShortcutEntry> shortcutsToCreate)
        {
            string lnkFileName;
            int count = 0;
            foreach (ShortcutEntry shortcutToCreate in shortcutsToCreate)
            {
                this.onCurrentActivityChanged("Shortcut " + shortcutToCreate.name);
                try
                {
                    count++;
                    this.calculateProgress(CurrentStepNumber, shortcutsToCreate.Count, count);
                    lnkFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), shortcutToCreate.name + ".lnk");
                    Shortcut.Create(lnkFileName, shortcutToCreate.path, null, Path.GetDirectoryName(shortcutToCreate.path), "Open " + shortcutToCreate.name, "", shortcutToCreate.path);

                    if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", shortcutToCreate.name)))
                    {
                        Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", shortcutToCreate.name));
                    }
                    lnkFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", shortcutToCreate.name, shortcutToCreate.name + ".lnk");
                    Shortcut.Create(lnkFileName, shortcutToCreate.path, null, Path.GetDirectoryName(shortcutToCreate.path), "Open " + shortcutToCreate.name, "", shortcutToCreate.path);
                    Console.WriteLine("{0} {1}", shortcutToCreate.path, " shortcut created");
                }
                catch
                {

                }
            }
        }

        private void ExecuteFiles(List<ApplicationToRun> files)
        {
            int count = 0;
            foreach (ApplicationToRun application in files)
            {
                string file = application.applicationPath;
                bool waitForExit = application.waitForApplicationToEnd;
                this.onCurrentActivityChanged("Ejecutando " + file);
                if (System.IO.File.Exists(file))
                {
                    ProcessStartInfo processInfo;
                    processInfo = new ProcessStartInfo(file);
                    processInfo.WorkingDirectory = Path.GetDirectoryName(file);
                    processInfo.UseShellExecute = true;
                    processInfo.Arguments = application.arguments;
                    if (application.visible == false)
                    {
                        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }
                    else
                    {
                        processInfo.WindowStyle = ProcessWindowStyle.Normal;
                    }
                    count++;
                    this.calculateProgress(CurrentStepNumber, files.Count, count);
                    Process process;
                    process = Process.Start(processInfo);
                    Console.WriteLine("{0} {1}", process.ProcessName, " executed");
                    if (waitForExit) process.WaitForExit(20000);
                }
            }
        }

        /// <summary>
        /// Run the required services
        /// </summary>
        /// <param name="services"></param>
        private void RunServices(ArrayList services)
        {
            int count = 0;
            foreach (string service in services)
            {
                ServiceController sc = new ServiceController();
                sc.ServiceName = service;
                sc.MachineName = Environment.MachineName;
                sc.Refresh();
                this.onCurrentActivityChanged("Ejecutando " + service);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    try
                    {
                        count++;
                        this.calculateProgress(CurrentStepNumber, services.Count, count);
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    finally { }
                }
            }
        }

        private void AddValuesToRegistry(List<RegistryEntry> registryEntriesToAdd)
        {
            int count = 0;
            foreach (RegistryEntry entry in registryEntriesToAdd)
            {
                this.onCurrentActivityChanged("Registro: " + entry.path + " Key=" + entry.key + "=" + entry.value);
                count++;
                try
                {
                    this.calculateProgress(CurrentStepNumber, registryEntriesToAdd.Count, count);
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(entry.path, true);
                    if (rk == null) rk = Registry.LocalMachine.CreateSubKey(entry.path, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    rk.SetValue(entry.key, entry.value);
                }
                catch(Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Clear all the items in the array lists
        /// </summary>
        public void Clear()
        {
            servicesToStop.Clear();
            processesToStop.Clear();
            filesToDelete.Clear();
            filesToCopy.Clear();
            appsToExecute.Clear();
            servicesToRun.Clear();
            registryEntriesToAdd.Clear();
        }

        private STATE CurrentState
        {
            get
            {
                return this.currentState;
            }
            set
            {
                this.currentState = value;
                this.OnStateChanged(this.currentState);
            }
        }

        private void OnStateChanged(STATE state)
        {
            if (StateChanged != null) StateChanged(state);
        }
    }

}
