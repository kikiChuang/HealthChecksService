using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.Data
{
    internal class HealthCheckConfiguration
    {
        public int Id { get; set; }
        /// <summary>
        /// 健康接口
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string Name { get; set; }
        public string DiscoveryService { get; set; }
        /// <summary>
        /// GUID
        /// </summary>
        public string HealthGuid { get; set; }
        /// <summary>
        /// 員編
        /// </summary>
        public string Userstfn { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string description { get; set; }
        public void Deconstruct(out string uri, out string name)
        {
            uri = this.Uri;
            name = this.Name;
        }
    }
}
