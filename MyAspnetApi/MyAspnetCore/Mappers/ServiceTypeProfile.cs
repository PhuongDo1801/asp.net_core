using AutoMapper;
using MyAspnetCore.DTO.ServiceType;
using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Mappers
{
    public class ServiceTypeProfile : Profile
    {
        public ServiceTypeProfile()
        {
            CreateMap<ServiceType, ServiceTypeDto>();
            CreateMap<ServiceTypeCreateDto, ServiceType>();
            CreateMap<ServiceTypeUpdateDto, ServiceType>();
        }   
    }
}
