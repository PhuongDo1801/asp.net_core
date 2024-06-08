using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Infrastructure
{
    public interface IActivityLogRepository : IBaseRepository<ActivityLog>
    {
        Task<IEnumerable<ActivityLog>> GetLogById(string id);
    }
}
