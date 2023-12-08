using MyAspnetCore.DTO.ServiceType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IServiceTypeService : IBaseService<ServiceTypeDto, ServiceTypeCreateDto, ServiceTypeUpdateDto>
    {
    }
}
