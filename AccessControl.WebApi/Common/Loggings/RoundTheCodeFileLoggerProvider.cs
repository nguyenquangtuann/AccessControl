using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace AccessControl.WebApi.Common.Loggings
{
    [ProviderAlias("RoundTheCodeFile")]
    public class RoundTheCodeFileLoggerProvider : ILoggerProvider
    {
        public readonly RoundTheCodeFileLoggerOption Options;

        public RoundTheCodeFileLoggerProvider(IOptions<RoundTheCodeFileLoggerOption> _options)
        {
            Options = _options.Value;
            string path = AppDomain.CurrentDomain.BaseDirectory+"\\"+ Options.FolderPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new RoundTheCodeFileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}