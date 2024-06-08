using MyAspnetCore.DTO.ActivityLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IActivityLogService : IBaseService<ActivityLogDto, ActivityCreateDto, ActivityLogUpdateDto>
    {
        public Task<IEnumerable<ActivityLogDto>> GetLogById(string id);
    }
}
