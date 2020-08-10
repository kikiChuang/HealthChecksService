using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.HostedService
{
    interface IHealthCheckNotifyCollector
    {
        Task Collect(CancellationToken cancellationToken);
    }
}
