using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.Service;
using MyAspnetCore.DTO.User;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ServicesController : BaseController<ServiceDto, ServiceCreateDto, ServiceUpdateDto>
    {
        private readonly IServiceService _serviceService;
        public ServicesController(IServiceService serviceService) : base(serviceService)
        {
            _serviceService = serviceService;
        }
        [HttpPost("ByServiceCode")]
        public async Task<IActionResult> InsertAsyncGetDetail(string serviceCode, Guid providerId)
        {
            var res = await _serviceService.InsertAsyncGetDetail(serviceCode, providerId);
            return StatusCode(StatusCodes.Status200OK, res);
        }
        [HttpGet("Filter")]
        public async Task<IActionResult> GetList([FromQuery] string? querySearch, [FromQuery] int? recordsPerPage, [FromQuery] int? page)
        {

            IEnumerable<ServiceDto> users;
            int totalRecord = 0;
            if (querySearch == null)
            {
                querySearch = "";
            }
            querySearch = querySearch.Trim();

            (totalRecord, users) = await _serviceService.GetListAsync(querySearch, recordsPerPage, page);

            return StatusCode(StatusCodes.Status200OK, new
            {
                TotalRecord = totalRecord,
                Data = users
            });
        }
    }
}
