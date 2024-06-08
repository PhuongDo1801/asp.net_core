using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.ActivityLog;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ActivityLogController : BaseController<ActivityLogDto, ActivityCreateDto, ActivityLogUpdateDto>
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService) : base(activityLogService)
        {
            _activityLogService = activityLogService;
        }
        [HttpGet("GetLog")]
        public async Task<IActionResult> GetLogById(string id)
        {
            var res = await _activityLogService.GetLogById(id);
            return StatusCode(StatusCodes.Status200OK, res);
        }
    }
}
