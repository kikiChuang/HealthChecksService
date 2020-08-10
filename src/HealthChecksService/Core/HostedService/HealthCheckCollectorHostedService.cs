using HealthCheckHostServer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;



namespace HealthCheckHostServer.Core.HostedService
{
    internal class HealthCheckCollectorHostedService : IHostedService
    {
        private readonly ILogger<HealthCheckCollectorHostedService> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceProvider _serviceProvider;
        private readonly Settings _settings;
        private CancellationTokenSource _cancellationTokenSource;
        private int count = 1;
        private Timer _timer;

        public HealthCheckCollectorHostedService
            (IServiceProvider provider,
            IOptions<Settings> settings,
            ILogger<HealthCheckCollectorHostedService> logger,
            IHostApplicationLifetime lifetime)
        {
            _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(provider));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _settings = settings.Value ?? new Settings();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Execute, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;

        }
        private Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await Collect(cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // We are halting, task cancellation is expected.
                }
            });

            return Task.CompletedTask;
        }

        private async void Execute(object state)
        {
            try
            {
                // 一天一循環 歸零。 4 * 60 * 24
                if (count == 5760)
                    count = 0;

                _logger.LogWarning(DateTime.Now.ToLongTimeString() + ": Execute!");
                await ExecuteAsync(_cancellationTokenSource.Token);

                count++;

            }
            catch (TaskCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                _logger.LogDebug("Execute exception");
            }
            finally
            {
                _logger.LogDebug("Execute finished");
            }
        }

        private async Task Collect(CancellationToken cancellationToken)
        {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            // Collect should not be triggered until IServerAddressFeature reports the listening endpoints

            _logger.LogDebug("Executing HealthCheck collector HostedService.");

            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var runner = scope.ServiceProvider.GetRequiredService<IHealthCheckReportCollector>();
                    await runner.Collect(_settings.EvaluationTimeInSeconds * count, cancellationToken);

                    _logger.LogDebug("HealthCheck collector HostedService executed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HealthCheck collector HostedService threw an error: {Error}", ex.Message);
                }
            }
        }
    }
}