using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Infrastructure
{
    public interface IUserRepository : IBaseRepository<User>
        ///Khai báo các hàm của User
    {
        Task<(int, IEnumerable<User>)> GetListAsync(string? queryName, int? recordsPerPage, int? page);

    }
}
