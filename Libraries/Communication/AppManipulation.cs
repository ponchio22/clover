using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;    
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace Valutech.IO
{    
    public class AppManipulation
    {
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
                
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, UIntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
         
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enable);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindowAsync")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        #region "Refresh Notification Area Icons"

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public static void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }


        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }
        #endregion

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public const uint WM_SETTEXT = 0x000C;

        public const uint BM_CLICK = 0x00F5;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;
        private const int WS_SHOWNORMAL = 1;
        private const int SW_HIDE = 0;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        public List<IntPtr> list;

        public String stringList;

        public static bool IsWindowMinimized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(hWnd, ref placement);
            return (placement.showCmd == 2);
        }
        public static IntPtr FindWindowByClassName(string className) 
        {
            IntPtr hwnd = FindWindow(className, null);
            return hwnd;
        }

        public static IntPtr FindWindowByWindowName(string windowName)
        {
            IntPtr hwnd = FindWindow(null, windowName);            
            return hwnd;
        }


        public static IntPtr FindChildWindowByWindowName(IntPtr parenthWnd, string windowName)
        {
            return FindWindowEx(parenthWnd, IntPtr.Zero, null,windowName);
        }

        public static IntPtr FindChildWindowByClassName(IntPtr parenthWnd, string className)
        {   
            List<IntPtr> list = AppManipulation.GetChildWindows(parenthWnd);
            string listString = String.Empty;
            int nRet;
            IntPtr match = IntPtr.Zero;
            int count = 0;
            foreach (IntPtr item in list)
            {
                StringBuilder ClassName = new StringBuilder(100);
                nRet = AppManipulation.GetClassName(item, ClassName, ClassName.Capacity);
                if (nRet != 0)
                {
                    if (className.ToString() == className)
                    {
                        if(match == IntPtr.Zero) match = item;                                                
                        Console.WriteLine(count.ToString() + " " + className + " " + item.ToString("X8"));
                        count++;
                    }
                }
            }
            return match;            
        }

        public static int FindChildWindowCountByClassName(IntPtr parenthWnd, string className)
        {
            List<IntPtr> list = AppManipulation.GetChildWindows(parenthWnd);            
            return list.Count;
        }

        public static IntPtr FindChildWindowByClassNameByIndex(IntPtr parenthWnd, string className,int index)
        {
            List<IntPtr> list = AppManipulation.GetChildWindows(parenthWnd);
            if (index < list.Count)
            {
                return list[index];
            }
            return IntPtr.Zero;
        }

        public static IntPtr FindLastChildWindowByClassName(IntPtr parenthWnd, string className)
        {
            List<IntPtr> list = AppManipulation.GetChildWindows(parenthWnd);
            if (list.Count > 0)
            {
                return list[list.Count - 1];
            }
            return IntPtr.Zero;
        }

        public static void SendClickToWindow(IntPtr hWnd)
        {            
            PostMessage(hWnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        }

        public static void FocusOnWindow(IntPtr hWnd)
        {            
            /*
            ShowWindow(hWnd, SW_MINIMIZE);
            ShowWindow(hWnd, SW_SHOWNORMAL);
            SetForegroundWindow(hWnd);
            SetFocus(hWnd);                                    
            SwitchToThisWindow(hWnd, true);
            SetActiveWindow(hWnd);
             * */
            IntPtr focusedWnd = GetForegroundWindow();
            SetWindowPos(focusedWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            ShowWindow(hWnd, SW_SHOWNORMAL);
            SetForegroundWindow(hWnd);
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            /*
            SetForegroundWindow(hWnd);
            SetFocus(hWnd);
            SwitchToThisWindow(hWnd, true);
            SetActiveWindow(hWnd);
             * */
        }

        public static bool HasFocus(IntPtr hWnd)
        {
            IntPtr focusedWnd = GetForegroundWindow();
            return (focusedWnd == hWnd);
        }

        public static void MaximizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_MAXIMIZE);
        }

        public static void RunAppAndKillOtherInstances(string processPath, string processName,string extension)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                string path = Path.Combine(processPath, processName + extension);
                if (processes.Length > 0)
                {
                    string output = String.Empty;
                    foreach (Process process in processes)
                    {
                        if (process.Modules[0].FileName != path)
                        {
                            process.Kill();
                        }
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        ProcessStartInfo processInfo;
                        processInfo = new ProcessStartInfo(path);
                        processInfo.WorkingDirectory = Path.GetDirectoryName(path);
                        processInfo.UseShellExecute = true;                        
                        processInfo.WindowStyle = ProcessWindowStyle.Normal;
                        Process.Start(processInfo);
                    }
                }
            }
            catch
            {
            }
        }

        public static string GetText(IntPtr hWnd)
        {
            const uint WM_GETTEXT = 13;
            const int bufferSize = 1000; // adjust as necessary
            StringBuilder sb = new StringBuilder(bufferSize);
            SendMessage(hWnd, WM_GETTEXT, new UIntPtr(bufferSize), sb);
            return sb.ToString();
        }

        public static string GetWindowName(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(256);
            if (GetWindowText(hWnd, title, 256) > 0)
            {
                return title.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            return true;
        }

        public static void SetText(IntPtr hWnd, string text)
        {
            SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, text);            
        }

        public static void SendTab(IntPtr hWnd)
        {
            SendMessage(hWnd, 260, 9, 0);
        }

        public static void WriteInNotepad(string text)
        {
            AppManipulation.SetText(AppManipulation.FindChildWindowByClassName(AppManipulation.FindWindowByWindowName("Untitled - Notepad"), "Edit"), text);
        }

        public static void SetEnabled(IntPtr hWnd, bool enabled)
        {
            EnableWindow(hWnd, enabled);
        }

        public static bool CheckIfProcessIsRunning(string nameSubstring)
        {
            return Process.GetProcesses().Any(p => p.ProcessName.Contains(nameSubstring));
        }
    }
}
