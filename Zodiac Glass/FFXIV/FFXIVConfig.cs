using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZodiacGlass.FFXIV
{
    class FFXIVConfig : IDisposable
    {
        private readonly StreamReader stream;

        public FFXIVConfig()
        {
            stream = new StreamReader(File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\FINAL FANTASY XIV - A Realm Reborn\FFXIV.cfg"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
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
                    {
                        return (ScreenMode)ushort.Parse(line.Substring(line.Length - 1));
                    }
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
