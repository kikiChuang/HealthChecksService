using HealthChecks.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    internal class HealthCheckExecution
    {
        public int Id { get; set; }
        /// <summary>
        /// 健康狀態
        /// </summary>
        public UIHealthStatus Status { get; set; }
        /// <summary>
        /// 健康狀態持續時間
        /// </summary>
        public DateTime OnStateFrom { get; set; }
        /// <summary>
        /// 上次執行時間
        /// </summary>
        public DateTime LastExecuted { get; set; }
        /// <summary>
        /// 健康接口
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string Name { get; set; }

        public string DiscoveryService { get; set; }

        public List<HealthCheckExecutionEntry> Entries { get; set; }

        public List<HealthCheckExecutionHistory> History { get; set; }
    }
}
