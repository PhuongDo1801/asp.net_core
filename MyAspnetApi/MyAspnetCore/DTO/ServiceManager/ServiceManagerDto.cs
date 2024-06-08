using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.ServiceManager
{
    public class ServiceManagerDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public int Status { get; set; }
    }
}
