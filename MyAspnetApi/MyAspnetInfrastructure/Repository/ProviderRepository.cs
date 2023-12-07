using Microsoft.Extensions.Configuration;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetInfrastructure.Repository
{
    public class ProviderRepository : BaseRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
