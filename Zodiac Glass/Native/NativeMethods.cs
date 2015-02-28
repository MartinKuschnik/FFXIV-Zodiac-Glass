namespace ZodiacGlass.Native
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        private static class Internal
        {
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong); // only x86 supported

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
            internal static extern IntPtr GetWindowLong(IntPtr hWnd, WindowLong nIndex);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAcess, bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, out int lpNumberOfBytesRead);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern bool CloseHandle(IntPtr hObject);
        }

        internal static IntPtr GetWindowLong(IntPtr hWnd, WindowLong nIndex)
        {
            IntPtr p = NativeMethods.Internal.GetWindowLong(hWnd, nIndex);

            if (p == IntPtr.Zero)
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }

            return p;
        }

        internal static IntPtr OpenProcess(ProcessAccessFlags dwDesiredAcess, bool bInheritHandle, int dwProcessId)
        {
            IntPtr p = NativeMethods.Internal.OpenProcess(dwDesiredAcess, bInheritHandle, dwProcessId);

            if (p == IntPtr.Zero)
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }

            return p;
        }


        internal static bool TryReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref byte[] lpBuffer, int iSize, out int lpNumberOfBytesRead)
        {
            return NativeMethods.Internal.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, iSize, out lpNumberOfBytesRead);
        }

        internal static void CloseHandle(IntPtr hObject)
        {
            if (!NativeMethods.Internal.CloseHandle(hObject))
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }
        }

        internal static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent)
        {
            IntPtr p = NativeMethods.Internal.SetParent(hWndChild, hWndNewParent);

            if (p == IntPtr.Zero)
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }

            return p;
        }

        internal static IntPtr SetWindowLong(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong)
        {
            IntPtr p = NativeMethods.Internal.SetWindowLong(hWnd, nIndex, dwNewLong);

            if (p == IntPtr.Zero)
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }

            return p;
        }

        internal static void SetForegroundWindow(IntPtr hWnd)
        {
            if (!NativeMethods.Internal.SetForegroundWindow(hWnd))
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }
        }

        internal static RECT GetWindowRect(IntPtr hwnd)
        {
            RECT rect;

            if (!NativeMethods.Internal.GetWindowRect(hwnd, out rect))
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }

            return rect;
        }

        internal static void SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags)
        {
            if (!NativeMethods.Internal.SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, flags))
            {
                int errorCoder = Marshal.GetLastWin32Error();

                if (errorCoder != 0)
                    throw new Win32Exception(errorCoder);
            }
        }
    }
}
