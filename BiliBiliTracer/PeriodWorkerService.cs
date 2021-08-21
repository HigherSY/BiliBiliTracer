using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BiliBiliTracer
{
    public abstract class PeriodWorkerService : BackgroundService
    {
        protected abstract TimeSpan Delay { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWorkAsync();

                await Task.Delay(Delay, stoppingToken);
            }
        }

        protected abstract Task DoWorkAsync();
    }
}
