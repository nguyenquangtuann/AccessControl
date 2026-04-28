using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AccessControl.WebApi.Common.Loggings
{
    public static class RoundTheCodeFileLoggerExtension
    {
        public static ILoggingBuilder AddRoundTheCodeFileLogger(this ILoggingBuilder builder, Action<RoundTheCodeFileLoggerOption> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, RoundTheCodeFileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}