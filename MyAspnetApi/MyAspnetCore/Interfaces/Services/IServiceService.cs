using MyAspnetCore.DTO.Service;
using MyAspnetCore.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IServiceService : IBaseService<ServiceDto, ServiceCreateDto, ServiceUpdateDto>
    {
        public Task<ServiceCreateDto> getServiceDetails(string serviceCode, Guid providerId);
        public Task<int> InsertAsyncGetDetail(string serviceCode, Guid providerId);
        public Task<(int, IEnumerable<ServiceDto>)> GetListAsync(string? queryName, int? recordsPerPage, int? page);
    }
}
