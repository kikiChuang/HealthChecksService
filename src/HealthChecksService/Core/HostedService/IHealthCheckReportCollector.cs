using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.HostedService
{
    interface IHealthCheckReportCollector
    {
        Task Collect(int callTimes ,CancellationToken cancellationToken);
    }
}
