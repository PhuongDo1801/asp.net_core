using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.Service;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : BaseController<ServiceDto, ServiceCreateDto, ServiceUpdateDto>
    {
        private readonly IServiceService _serviceService;
        public ServicesController(IServiceService serviceService) : base(serviceService)
        {
            _serviceService = serviceService;
        }
    }
}
