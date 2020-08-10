using HealthChecks.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    public class HealthCheckExecutionEntry
    {
        public int Id { get; set; }
        /// <summary>
        /// 檢查物件名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 健康狀態
        /// </summary>
        public UIHealthStatus Status { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        //public bool Notify { get; set; } = false;

        //public string RunAt { get; set; }

        //public string ExecuteAt { get; set; }
        //public string State { get; set; } = "0";

        public int HealthCheckExecutionId { get; set; }
        /// <summary>
        /// 執行時間
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
