using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    internal class HealthCheckParameter
    {
        public int Id { get; set; }
        /// <summary>
        /// C槽容量設定
        /// </summary>
        public string C_disk { get; set; }
        /// <summary>
        /// D槽容量設定
        /// </summary>
        public string D_disk { get; set; }
        /// <summary>
        /// E槽容量設定
        /// </summary>
        public string E_disk { get; set; }
        /// <summary>
        /// F槽容量設定
        /// </summary>
        public string F_disk { get; set; }
        /// <summary>
        /// G槽容量設定
        /// </summary>
        public string G_disk { get; set; }
        /// <summary>
        /// 記憶體容量設定
        /// </summary>
        public string memory { get; set; }
        /// <summary>
        /// 是否推播通知
        /// </summary>
        public string notify { get; set; }
        /// <summary>
        /// Webhook Link
        /// </summary>
        public string webhook { get; set; }
        public DateTime On { get; set; }
        public int ConfigID { get; set; }
        /// <summary>
        /// 執行檢查時間
        /// </summary>
        public int ExecutionTime { get; set; }
    }
}
