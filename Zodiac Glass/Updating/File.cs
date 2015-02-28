namespace ZodiacGlass.Updating
{
    using System;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("{Source,nq}")]
    public class File
    {
        public string Source { get; set; }

        public string Target { get; set; }
    }

}
