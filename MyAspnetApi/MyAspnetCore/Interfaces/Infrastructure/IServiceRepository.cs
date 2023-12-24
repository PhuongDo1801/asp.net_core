using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Infrastructure
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task<(int, IEnumerable<Service>)> GetListAsync(string? queryName, int? recordsPerPage, int? page);
    }
}
