using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.DTO.Provider;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProvidersController : BaseController<ProviderDto, ProviderCreateDto, ProviderUpdateDto>
    {
        private readonly IProviderService _providerService;

        public ProvidersController(IProviderService providerService) : base(providerService)
        {
            _providerService = providerService;
        }
    }
}
