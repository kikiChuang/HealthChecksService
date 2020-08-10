using HealthChecks.UI.Core;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Notifications
{
    interface IHealthCheckFailureNotifier
    {
        Task NotifyDown(int id, string name, UIHealthReport report);
        Task NotifyWakeUp(int id, string name);

    }
}
