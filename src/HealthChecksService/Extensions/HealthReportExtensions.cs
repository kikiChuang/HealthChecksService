using HealthCheckHostServer.Core.Data;
using HealthChecks.UI.Core;
using System.Collections.Generic;
using System.Linq;

namespace HealthCheckHostServer.Extensions
{
    public static class HealthReportExtensions
    {
        public static List<HealthCheckExecutionEntry> ToExecutionEntries(this UIHealthReport report)
        {
            return report.Entries
                .Select(item =>
                {
                    return new HealthCheckExecutionEntry()
                    {
                        Name = item.Key,
                        Status = item.Value.Status,
                        Description = item.Value.Description,
                        Duration = item.Value.Duration
                    };
                }).ToList();
        }
    }
}
