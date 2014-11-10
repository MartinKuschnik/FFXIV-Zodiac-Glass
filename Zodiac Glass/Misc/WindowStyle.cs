using System;
namespace ZodiacGlass
{
    [Flags]
    public enum WindowStyle : int
    {
        WS_OVERLAPPED = 0,
        WS_POPUP = -2147483648,
        WS_CHILD = 1073741824,
        WS_MINIMIZE = 536870912,
        WS_VISIBLE = 268435456,
        WS_DISABLED = 134217728,
        WS_CLIPSIBLINGS = 67108864,
        WS_CLIPCHILDREN = 33554432,
        WS_MAXIMIZE = 16777216,
        WS_BORDER = 8388608,
        WS_DLGFRAME = 4194304,
        WS_CAPTION = WindowStyle.WS_BORDER | WindowStyle.WS_DLGFRAME,
        WS_VSCROLL = 2097152,
        WS_HSCROLL = 1048576,
        WS_SYSMENU = 524288,
        WS_THICKFRAME = 262144,
        WS_TABSTOP = 65536,
        WS_MINIMIZEBOX = 131072,
        WS_MAXIMIZEBOX = 65536,
    }
}
