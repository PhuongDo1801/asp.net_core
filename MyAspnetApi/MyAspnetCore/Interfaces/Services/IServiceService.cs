using MyAspnetCore.DTO.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IServiceService : IBaseService<ServiceDto, ServiceCreateDto, ServiceUpdateDto>
    {
    }
}
