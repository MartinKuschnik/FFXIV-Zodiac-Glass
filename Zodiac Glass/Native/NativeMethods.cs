namespace ZodiacGlass.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAcess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, WindowLong nIndex);

        private static class Internal
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong);
        }


        public static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return NativeMethods.Internal.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }

            return NativeMethods.Internal.SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }
}
