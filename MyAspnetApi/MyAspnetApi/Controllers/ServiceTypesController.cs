using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.ServiceType;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTypesController : BaseController<ServiceTypeDto, ServiceTypeCreateDto, ServiceTypeUpdateDto>
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypesController(IServiceTypeService serviceTypeService) : base(serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }
    }
}
