namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class FFXIVConfigReader : IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly StreamReader stream;

        public FFXIVConfigReader(string path)
        {
            this.stream = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        public FFXIVConfig ReadConfig()
        {
            FFXIVConfig config = default(FFXIVConfig);

            this.stream.BaseStream.Position = 0;

            while (!this.stream.EndOfStream)
            {
                string line = stream.ReadLine();

                if (line.StartsWith("ScreenMode"))
                {
                    config.ScreenMode = (FFXIVScreenMode)ushort.Parse(line.Substring(line.Length - 1));                    
                }
            }

            return config;
        }

        public void Dispose()
        {
            this.stream.Dispose();
        }
    }
}
