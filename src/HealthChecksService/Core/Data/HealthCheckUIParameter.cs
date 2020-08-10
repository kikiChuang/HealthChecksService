using System;


namespace HealthCheckHostServer.Core.Data
{
    internal class HealthCheckUIParameter
    {
        public int Id { get; set; }
        /// <summary>
        /// service執行時間
        /// </summary>
        public string OnRunCallAPITime { get; set; }
        /// <summary>
        /// 推播通知時間
        /// </summary>
        public string OnNotifyPushTime { get; set; }
        /// <summary>
        /// 上次執行時間
        /// </summary>
        public DateTime LastDateTime { get; set; }

    }
}
