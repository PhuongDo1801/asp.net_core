using MyAspnetCore.DTO.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IServiceManagerService : IBaseService<ServiceManagerDto, ServiceManagerCreateDto, ServiceManagerUpdateDto>
    {
    }
}
