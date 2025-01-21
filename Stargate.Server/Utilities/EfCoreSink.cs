using Serilog.Core;
using Serilog.Events;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

public class EfCoreSink : ILogEventSink
{
    private readonly IServiceProvider _serviceProvider;

    public EfCoreSink(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<StargateContext>();

            var log = new Logs
            {
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                CreatedDate = DateTime.Now
            };

            dbContext.Logs.Add(log);
            dbContext.SaveChanges();
        }
    }
}
