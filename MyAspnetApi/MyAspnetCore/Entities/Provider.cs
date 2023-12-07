using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class Provider : BaseEntity
    {
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; }   
        public string Description { get; set; }
    }
}
