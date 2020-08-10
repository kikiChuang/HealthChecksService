using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HealthCheckHostServer;
using HealthCheckHostServer.Configuration;
using HealthCheckHostServer.Core;
using HealthCheckHostServer.Core.Data;
using HealthCheckHostServer.Core.HostedService;
using HealthCheckHostServer.Core.Notifications;
using HealthChecksService.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HealthChecksService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   //Task Service
                   services.AddHealthChecksTaskService();
                });
    }
}
