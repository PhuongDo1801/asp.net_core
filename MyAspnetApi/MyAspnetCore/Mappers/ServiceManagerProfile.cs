using AutoMapper;
using MyAspnetCore.DTO.Service;
using MyAspnetCore.DTO.ServiceManager;
using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Mappers
{
    public class ServiceManagerProfile : Profile
    {
        public ServiceManagerProfile()
        {
            CreateMap<ServiceManager, ServiceManagerDto>();
            CreateMap<ServiceManagerCreateDto, ServiceManager>();
            CreateMap<ServiceManagerUpdateDto, ServiceManager>();
        }
    }
}
