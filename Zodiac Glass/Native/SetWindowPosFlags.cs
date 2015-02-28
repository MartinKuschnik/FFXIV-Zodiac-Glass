namespace ZodiacGlass.Native
{
    using System;

    [Flags]
    public enum SetWindowPosFlags
    {
        SWP_NONE = 0,
        SWP_NOSIZE = 1,
        SWP_NOMOVE = 2,
        SWP_NOZORDER = 4,
        SWP_NOREDRAW = 8,
        SWP_NOACTIVATE = 16,
        SWP_FRAMECHANGED = 32,
        SWP_SHOWWINDOW = 64,
        SWP_HIDEWINDOW = 128,
        SWP_NOCOPYBITS = 256,
        SWP_NOOWNERZORDER = 512,
        SWP_NOSENDCHANGING = 1024,
        SWP_DEFERERASE = 8192,
        SWP_ASYNCWINDOWPOS = 16384
    }
}
