using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetInfrastructure.Repository;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.DTO.User;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController<UserDto, UserCreateDto, UserUpdateDto>
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> GetList([FromQuery] string? querySearch, [FromQuery] int? recordsPerPage, [FromQuery] int? page)
        {

            IEnumerable<UserDto> users;
            int totalRecord = 0;
            if (querySearch == null)
            {
                querySearch = "";
            }
            querySearch = querySearch.Trim();

            (totalRecord, users) = await _userService.GetListAsync(querySearch, recordsPerPage, page);

            return StatusCode(StatusCodes.Status200OK, new
            {
                TotalRecord = totalRecord,
                Data = users
            });
        }

            bool IsValidEmail(string email)
        {   
            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith(".")){
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch 
            {
                return false;
            }
        }
    }
}
