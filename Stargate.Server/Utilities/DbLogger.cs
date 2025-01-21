using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using Serilog;
using Stargate.Server.Data;
using ILogger = Serilog.ILogger;

namespace Stargate.Server.Utilities
{
    public static class DbLogger
    {
        private static ILogger? _logger;

        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var serviceProvider = new ServiceCollection()
                        .AddDbContext<StargateContext>(options => options.UseSqlite("Data Source=starbase.db"))
                        .AddSingleton<ILogEventSink, EfCoreSink>()
                        .BuildServiceProvider();

                    _logger = new LoggerConfiguration()
                        .WriteTo.Sink(serviceProvider.GetRequiredService<ILogEventSink>())
                        .CreateLogger();
                }

                return _logger;
            }
        }
    }
}
