using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class ServiceType : BaseEntity
    {
        public Guid ServiceTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
    }
}
