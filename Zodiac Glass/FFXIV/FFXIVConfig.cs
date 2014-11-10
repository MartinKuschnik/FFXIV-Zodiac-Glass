namespace ZodiacGlass.FFXIV
{
    using System;
    using System.IO;
    using System.Diagnostics;

    internal class FFXIVConfig : IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly StreamReader stream;

        public FFXIVConfig()
        {
            this.stream = new StreamReader(File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\FINAL FANTASY XIV - A Realm Reborn\FFXIV.cfg"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        public ScreenMode ScreenMode
        {
            get
            {
                this.stream.BaseStream.Position = 0;

                while (!this.stream.EndOfStream)
                {
                    string line = stream.ReadLine();

                    if (line.StartsWith("ScreenMode"))
                        return (ScreenMode)ushort.Parse(line.Substring(line.Length - 1));
                }

                throw new InvalidDataException();
            }
        }

        public void Dispose()
        {
            this.stream.Dispose();
        }
    }
}
