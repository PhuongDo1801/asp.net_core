using AutoMapper;
using MyAspnetCore.DTO.ServiceManager;
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
    public class ServiceManagerService : BaseService<ServiceManager, ServiceManagerDto, ServiceManagerCreateDto, ServiceManagerUpdateDto>, IServiceManagerService
    {
        private readonly IServiceManagerRepository _serviceManagerRepository;
        public ServiceManagerService(IServiceManagerRepository serviceManagerRepository, IMapper mapper) : base(serviceManagerRepository, mapper)
        {
            _serviceManagerRepository = serviceManagerRepository;
        }
    }
}
