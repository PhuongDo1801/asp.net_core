using AutoMapper;
using MyAspnetCore.DTO.Provider;
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
    public class ProviderService : BaseService<Provider, ProviderDto, ProviderCreateDto, ProviderUpdateDto>, IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        public ProviderService(IProviderRepository providerRepository, IMapper mapper) : base(providerRepository, mapper)
        {
            _providerRepository = providerRepository;
        }

    }
}
