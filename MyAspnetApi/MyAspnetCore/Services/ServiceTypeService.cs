using AutoMapper;
using MyAspnetCore.DTO.ServiceType;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Services
{
    public class ServiceTypeService : BaseService<ServiceType, ServiceTypeDto, ServiceTypeCreateDto, ServiceTypeUpdateDto>, IServiceTypeService
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;
        public ServiceTypeService(IServiceTypeRepository serviceTypeRepository, IMapper mapper) : base(serviceTypeRepository, mapper)
        {
            _serviceTypeRepository = serviceTypeRepository;
        }

    }
}
