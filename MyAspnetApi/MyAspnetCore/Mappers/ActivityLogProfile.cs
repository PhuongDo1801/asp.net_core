using AutoMapper;
using MyAspnetCore.DTO.ActivityLog;
using MyAspnetCore.DTO.Service;
using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Mappers
{
    public class ActivityLogProfile : Profile
    {
        public ActivityLogProfile()
        {
            CreateMap<ActivityLog, ActivityLogDto>();
            CreateMap<ActivityCreateDto, ActivityLog>();
            CreateMap<ActivityLogUpdateDto, ActivityLog>();
        }
    }
}
