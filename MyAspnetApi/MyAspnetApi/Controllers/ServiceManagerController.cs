using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.ServiceManager;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class ServiceManagerController : BaseController<ServiceManagerDto, ServiceManagerCreateDto, ServiceManagerUpdateDto>
    {
        private readonly IServiceManagerService _serviceManagerService;

        public ServiceManagerController(IServiceManagerService serviceManagerService) : base(serviceManagerService)
        {
            _serviceManagerService = serviceManagerService;
        }
    }
}
