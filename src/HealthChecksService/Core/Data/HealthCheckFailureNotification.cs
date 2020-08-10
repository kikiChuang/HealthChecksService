using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    public class HealthCheckFailureNotification
    {
        public int Id { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string HealthCheckName { get; set; }
        /// <summary>
        /// 上次通知時間
        /// </summary>
        public DateTime LastNotified { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUpAndRunning { get; set; }

    }
}
