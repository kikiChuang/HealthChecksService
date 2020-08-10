using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Configuration
{
    public class Settings
    {
        internal List<HealthCheckSetting> HealthChecks { get; set; } = new List<HealthCheckSetting>();
        internal List<WebHookNotification> Webhooks { get; set; } = new List<WebHookNotification>();

        internal int EvaluationTimeInSeconds { get; set; } 
        internal int NotifyEvaluationTimeInSeconds { get; set; } 
        //internal int MinimumSecondsBetweenFailureNotifications { get; set; } = 60 * 10;

        internal string HealthCheckDatabaseConnectionString { get; set; }
        internal Action<IServiceProvider, HttpClient> ApiEndpointHttpClientConfig { get; private set; }
        internal Func<IServiceProvider, HttpMessageHandler> ApiEndpointHttpHandler { get; private set; }
        internal Func<IServiceProvider, HttpMessageHandler> WebHooksEndpointHttpHandler { get; private set; }
        internal Action<IServiceProvider, HttpClient> WebHooksEndpointHttpClientConfig { get; private set; }



    }
    public class HealthCheckSetting
    {
        public string Name { get; set; }
        public string Uri { get; set; }
    }
    public class WebHookNotification
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Payload { get; set; }
        public string RestoredPayload { get; set; }
    }
}
