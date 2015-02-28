namespace ZodiacGlass.Updating
{
    using System;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("{Version,nq}")]
    public class Update
    {
        public string Version { get; set; }
        public File[] Content { get; set; }
    }

}
