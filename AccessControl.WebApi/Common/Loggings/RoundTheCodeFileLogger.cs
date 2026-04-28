using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace AccessControl.WebApi.Common.Loggings
{
    public class RoundTheCodeFileLogger : ILogger
    {
        protected readonly RoundTheCodeFileLoggerProvider _roundTheCodeLoggerFileProvider;

        public RoundTheCodeFileLogger([NotNull] RoundTheCodeFileLoggerProvider roundTheCodeLoggerFileProvider)
        {
            _roundTheCodeLoggerFileProvider = roundTheCodeLoggerFileProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var fullFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + _roundTheCodeLoggerFileProvider.Options.FolderPath + "/" + _roundTheCodeLoggerFileProvider.Options.FilePath.Replace("{date}", DateTime.Now.ToString("yyyyMMdd"));

            var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");

            var fileInfo = new FileInfo(fullFilePath);
            if (fileInfo.Exists)
            {
                try
                {
                    using (var writer = new StreamWriter(fullFilePath, true))
                    {
                        writer.WriteLine(logRecord);
                        writer.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                try
                {
                    using (FileStream fs = File.Create(fullFilePath)) { }
                    using (StreamWriter str = new StreamWriter(fullFilePath, true))
                    {
                        str.BaseStream.Seek(0, SeekOrigin.End);
                        str.WriteLine(logRecord);
                        str.Flush();
                        str.Close();
                        str.Dispose();
                        // File.AppendAllText(fullFilePath, addtext);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}