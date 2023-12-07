using AutoMapper;
using MyAspnetCore.DTO.Provider;
using MyAspnetCore.DTO.User;
using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Mappers
{
    public class ProviderProfile : Profile
    {
        public ProviderProfile()
        {
            CreateMap<Provider, ProviderDto>();
            CreateMap<ProviderCreateDto, Provider>();
            CreateMap<ProviderUpdateDto, Provider>();
        }
    }
}
