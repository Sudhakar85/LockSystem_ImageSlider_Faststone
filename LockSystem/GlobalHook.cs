using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LockSystem
{
    public class GlobalHook
    {
        #region WinAPI

        [DllImport("user32.dll", CharSet = CharSet.Auto,
           CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        #endregion

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private static HookProc keyboardHookHandler;

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        //public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc mouseHookHandler;
        
        /// <summary>
        /// Hook ID
        /// </summary>
        private static int keyBoardhookID = 0;
        private static int mousehookID = 0;

        public GlobalHook()
        {
            
        }
        public void AddKeyBoardHook()
        {
            keyboardHookHandler = new HookProc(HookKeyboardFunc);
            keyBoardhookID = SetKeybordHook(keyboardHookHandler);

        }


        private void GetLastError()
        {
            if (keyBoardhookID == 0)
            {
                //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                int errorCode = Marshal.GetLastWin32Error();
                //do cleanup
                Uninstall();
                //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                throw new Win32Exception(errorCode);
            }
        }

        public void AddMouseHook()
        {
            mouseHookHandler = new HookProc(MouseHookFunc);
            mousehookID = SetMouseHook(mouseHookHandler);
        }

        private static int SetKeybordHook(HookProc proc)
        {
            // 13 - Low Level Keyboard hook code
            return SetWindowsHookEx(13, proc, Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]), 0);
        }

        private static int SetMouseHook(HookProc proc)
        {
            // 14 - Low Level Mouse hook code
            int result = SetWindowsHookEx(14, proc, Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            return result;

        }

        public static int MouseHookFunc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //int vkCode = Marshal.ReadInt32(lParam);                
                //File.AppendAllText(@"C:\\temp\hook.log", "Mouse:" + vkCode.ToString());
                Lock();
                
            }

            return CallNextHookEx(mousehookID, nCode, wParam, lParam);
        }

        private static void Lock()
        {
            Uninstall();
            LockWorkStation();
            Application.Exit();
            
        }
        private int HookKeyboardFunc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Not Left shit key sine this is key storahe for software fastStone slider
                if (vkCode != 160)
                {
                    //File.AppendAllText(@"C:\\temp\hook.log", "KeyBoard:" + vkCode.ToString());
                    Lock();
                }
                return 1;

            }
            return CallNextHookEx(keyBoardhookID, nCode, wParam, lParam);
        }

        public static void Uninstall()
        {
            UnhookWindowsHookEx((int)keyBoardhookID);
            UnhookWindowsHookEx(mousehookID);
        }

        ~GlobalHook()
        {
            Uninstall();
        }
    }
}
