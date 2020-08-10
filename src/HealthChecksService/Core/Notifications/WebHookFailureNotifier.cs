using HealthCheckHostServer.Configuration;
using HealthCheckHostServer.Core.Data;
using HealthChecks.UI.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Notifications
{
    internal class WebHookFailureNotifier
            : IHealthCheckFailureNotifier
    {
        private readonly ILogger<WebHookFailureNotifier> _logger;
        private readonly Settings _settings;
        private readonly HealthChecksDb _db;
        private readonly HttpClient _httpClient;

        public WebHookFailureNotifier(
            HealthChecksDb db,
            IOptions<Settings> settings,
            ILogger<WebHookFailureNotifier> logger,
            IHttpClientFactory httpClientFactory)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _settings = settings.Value ?? new Settings();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory.CreateClient(Keys.HEALTH_CHECK_WEBHOOK_HTTP_CLIENT_NAME);

        }
        public async Task NotifyDown(int id, string name, UIHealthReport report)
        {
            await Notify(id, name, report, isHealthy: false);
        }
        public async Task NotifyWakeUp(int id, string name)
        {
            await Notify(id, name, isHealthy: true);
        }
        private async Task Notify(int id, string name, UIHealthReport report = null, bool isHealthy = false)
        {
            var healthChecks = await (from hel in _db.HealthCheckParameters
                                      join con in _db.Configurations on hel.ConfigID equals con.Id
                                      where hel.ConfigID == id
                                      select new
                                      {
                                          WebHook = hel.webhook,
                                          execTime = hel.ExecutionTime
                                      }).ToListAsync();

            if (!string.IsNullOrWhiteSpace(healthChecks.FirstOrDefault().WebHook))
            {
                // Save [Failures] 有推播再save
                await SaveNotification(new HealthCheckFailureNotification()
                {
                    LastNotified = DateTime.UtcNow,
                    HealthCheckName = name,
                    IsUpAndRunning = isHealthy
                });

                if (!isHealthy)
                {
                    var outcomes = report.Entries
                   .Where(w => w.Value.Status != UIHealthStatus.Healthy)
                   .Select(s => string.Format("．{0} － {1}", s.Key, report.Entries[s.Key].Description.Replace("\\", "")));

                    var content = JsonConvert.SerializeObject(new
                    {
                        themeColor = "0072C6",
                        title = name,
                        text = string.Join("\n\n", outcomes)

                    });              

                    Uri uri = new Uri(healthChecks.First().WebHook);

                    // push to teams
                    await SendRequest(uri, "Teams", content);
                }
            }
        }
        private async Task SaveNotification(HealthCheckFailureNotification notification)
        {
            if (notification != null)
            {
                await _db.Failures
                    .AddAsync(notification);

                await _db.SaveChangesAsync();
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

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
