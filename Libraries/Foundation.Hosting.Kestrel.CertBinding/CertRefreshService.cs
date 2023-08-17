using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace Foundation.Hosting.Kestrel.CertBinding;

public class CertRefreshService : IHostedService
{
    private const string Schedule = "0 1 * * *"; // run day at 1 am

    private readonly CertificationStore _store;
    private readonly CrontabSchedule _crontabSchedule;
    private readonly ILogger _log;

    public CertRefreshService(CertificationStore store, ILogger<CertRefreshService> log)
    {
        _log = log;
        _store = store;
        _crontabSchedule = CrontabSchedule.Parse(Schedule);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var next = DateTime.Now;
        _log.LogInformation("Certification refresh service started");

        _store.LoadCertificates();

        Task.Factory.StartNew(async () => await ScheduleLoop(cancellationToken),
            cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task ScheduleLoop(CancellationToken cancellationToken)
    {
        var next = DateTime.Now;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {               
                next = NextOccurrence(next);
                var delay = next.Subtract(DateTime.Now);

                _log.LogInformation($"Next certification download scheduled for: {next}");

                await Task.Delay(delay, cancellationToken);
                
                _store.LoadCertificates();
            }
        }
        finally
        {
            _log.LogWarning("Certification refresh service exited.");
        }
    }

    private DateTime NextOccurrence(DateTime baseDate)
    {
        var next = baseDate;
        do
        {
            next = _crontabSchedule.GetNextOccurrence(next);
        } while (next < DateTime.Now);

        return next;
    }
}