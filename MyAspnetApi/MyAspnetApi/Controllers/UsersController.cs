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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController<UserDto, UserCreateDto, UserUpdateDto>
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService userService, IConfiguration configuration) : base(userService)
        {
            _userService = userService;
            _configuration = configuration;
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

        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmail(email);
            return StatusCode(StatusCodes.Status200OK, user);
        }

    }
}
