
namespace ZodiacGlass.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class Log : IDisposable
    {
        private readonly Lazy<StreamWriter> streamWriter;

        public Log()
        {
            this.streamWriter = new Lazy<StreamWriter>(this.CreateStreamWriter, true);
        }

        private StreamWriter CreateStreamWriter()
        {
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), string.Format("ZodiacGlass\\logs\\{0}.log", DateTime.Now.ToShortDateString()));
            string logFileDirectory = Path.GetDirectoryName(logFilePath);

            if (!Directory.Exists(logFileDirectory))
                Directory.CreateDirectory(logFileDirectory);

            return new StreamWriter(new FileStream(logFilePath, FileMode.Append)) { AutoFlush = true };
        }

        public void Write(LogLevel level, object value)
        {
            try
            {
                if (this.streamWriter.Value != null)
                {
                    string text = string.Format("{0} {1} {2}", DateTime.Now, string.Format("[{0}]:", level.ToString()).PadRight(/* 5 max LogLevel name*/5 + 3, ' ') , value);
                    this.streamWriter.Value.WriteLine(text);
                    Trace.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }


        internal void WriteException(object value, Exception ex)
        {
            this.Write(LogLevel.Error, string.Format("{0} {1}", value, ex));
        }

        public void Dispose()
        {
            if (this.streamWriter.IsValueCreated && this.streamWriter.Value != null)
            {
                this.streamWriter.Value.Dispose();
            }
        }
    }
}
