using HealthChecks.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    public class HealthCheckExecutionHistory
    {
        public int Id { get; set; }
        /// <summary>
        /// 檢查物件名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 健康狀態
        /// </summary>
        public UIHealthStatus Status { get; set; }
        /// <summary>
        /// 上次執行時間
        /// </summary>
        public DateTime On { get; set; }
    }
}
