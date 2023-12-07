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

        public Guid ServiceTypeId { get; set; }

        public string ServiceName { get; set; }
    }
}
