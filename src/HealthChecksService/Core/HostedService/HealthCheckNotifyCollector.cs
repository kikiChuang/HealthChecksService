using HealthCheckHostServer.Configuration;
using HealthCheckHostServer.Core.Data;
using HealthCheckHostServer.Core.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.HostedService
{
    internal class HealthCheckNotifyCollector
        : IHealthCheckNotifyCollector
    {
        private readonly HealthChecksDb _db;
        private readonly Settings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HealthCheckNotifyCollector> _logger;

        private static readonly Dictionary<int, Uri> endpointAddresses = new Dictionary<int, Uri>();

        public HealthCheckNotifyCollector(
            HealthChecksDb db,
            IHealthCheckFailureNotifier healthCheckFailureNotifier,
            IOptions<Settings> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<HealthCheckNotifyCollector> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory.CreateClient(Keys.HEALTH_CHECK_HTTP_CLIENT_NAME);
        }


        public async Task Collect(CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("HealthReportCollector is collecting health checks results."))
            {

                var healthChecks = await (from hel in _db.HealthCheckParameters
                                          join con in _db.Configurations on hel.ConfigID equals con.Id
                                          join exc in _db.Executions on con.Name equals exc.Name
                                          join tet in _db.HealthCheckExecutionEntries on exc.Id equals tet.HealthCheckExecutionId
                                          select new
                                          {
                                              Notify = hel.notify,
                                              WebHook = hel.webhook,
                                              ConName = con.Name,
                                              ConUri = con.Uri,
                                              tetName = tet.Name,
                                              tetDesc = tet.Description,
                                              tetStatus = tet.Status,
                                              Id = exc.Id
                                          }).ToListAsync();


                var filterData = healthChecks.Where(w => w.Notify != "false" && w.WebHook != null && (int)w.tetStatus != 2).OrderBy(w => w.Id);
                var tempName = filterData.Select(s => s.ConName).Distinct();
                foreach (var projN in tempName)
                {
                    var data = filterData.Where(w => w.ConName == projN).Select(s => new { tName = s.tetName, tDesc = s.tetDesc, webHook = s.WebHook });
                    Uri uri = new Uri(data.First().webHook);
                    await SendRequest(uri, "Teams", "{\r\n  \"@context\": \"http://schema.org/extensions\",\r\n  \"@type\": \"MessageCard\",\r\n  \"themeColor\": \"0072C6\",\r\n  \"title\": \"" + projN + "\",\r\n  \"text\": \"" + string.Join("\n\n", data.Select(s => "．" + s.tName + "－" + s.tDesc)) + "\",\r\n  \"potentialAction\":[]\r\n}");
                }

                _logger.LogDebug("HealthReportCollector has completed.");
            }
        }

        private async Task SendRequest(Uri uri, string name, string payloadContent)
        {
            try
            {
                var payload = new StringContent(payloadContent, Encoding.UTF8, Keys.DEFAULT_RESPONSE_CONTENT_TYPE);
                var response = await _httpClient.PostAsync(uri, payload);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("The webhook notification has not executed successfully for {name} webhook. The error code is {statuscode}.", name, response.StatusCode);
                }

            }
            catch (Exception exception)
            {
                _logger.LogError($"The failure notification for {name} has not executed successfully.", exception);
            }
        }


    }
}
