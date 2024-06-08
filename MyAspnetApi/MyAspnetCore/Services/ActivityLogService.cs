using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAspnetCore.DTO.ActivityLog;
using MyAspnetCore.DTO.Service;
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
    public class ActivityLogService : BaseService<ActivityLog, ActivityLogDto, ActivityCreateDto, ActivityLogUpdateDto>, IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        public ActivityLogService(IActivityLogRepository activityLogRepository, IMapper mapper) : base(activityLogRepository, mapper)
        {
            _activityLogRepository = activityLogRepository;
        }

        public async Task<IEnumerable<ActivityLogDto>> GetLogById(string id)
        {       
            var res = await _activityLogRepository.GetLogById(id);
            IEnumerable<ActivityLogDto> list = _mapper.Map<IEnumerable<ActivityLogDto>>(res);
            return list;
        }
    }
}
