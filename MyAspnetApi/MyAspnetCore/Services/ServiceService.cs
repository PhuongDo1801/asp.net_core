using AutoMapper;
using MyAspnetCore.DTO.Service;
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
    public class ServiceService : BaseService<Service, ServiceDto, ServiceCreateDto, ServiceUpdateDto>, IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        public ServiceService(IServiceRepository serviceRepository, IMapper mapper) : base(serviceRepository, mapper)
        {
            _serviceRepository = serviceRepository;
        }
    }
}
