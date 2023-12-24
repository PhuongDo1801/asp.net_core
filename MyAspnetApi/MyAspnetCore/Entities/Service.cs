using MyAspnetCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class Service : BaseEntity
    {
        public Service() { }

        public Guid ServiceId { get; set; }

        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public string ProductFamily { get; set; }
        public string EngineCode { get; set; }
        public string UsageType { get; set; }
        public string LocationType { get; set; }
        public string InstanceFamily { get; set; }
        public string Operation { get; set; }
        public string DatabaseEngine { get; set; }
        public string Description { get; set; }
    }
}
