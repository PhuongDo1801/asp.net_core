using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/ [controller]")]
    [ApiController]
    public class BaseController<TEntityDto, TEntityCreateDto, TEntityUpdateDto> : ControllerBase
    {
        #region property
        protected readonly IBaseService<TEntityDto, TEntityCreateDto, TEntityUpdateDto> _baseService;
        #endregion

        #region construct
        public BaseController(IBaseService<TEntityDto, TEntityCreateDto, TEntityUpdateDto> baseService)
        {
            _baseService = baseService;
        }
        #endregion

        #region api
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _baseService.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, result);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id)
        {
            var result = await _baseService.GetByIdAsync(Id);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        [HttpPost]
        public async Task<IActionResult> InsertAsync(TEntityCreateDto entityCreateDto)
        {
            var result = await _baseService.InsertAsync(entityCreateDto);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, TEntityUpdateDto entityUpdateDto)
        {
            var result = await _baseService.UpdateAsync(entityUpdateDto, Id);

            return StatusCode(StatusCodes.Status200OK, result);

        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _baseService.DeleteByIdAsync(Id);

            return StatusCode(StatusCodes.Status200OK, result);

        }
        #endregion
    }
}
