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

        [HttpGet("GetInfoUser"), Authorize]
        public async Task<IActionResult> GetInfoUser()
        {
            var userName = _userService.GetUserInfo();
            return Ok(userName.Result);
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
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto userCreateDto)
        {
            var result = await _userService.Register(userCreateDto);

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = new UserDto();
            var result = await _userService.Login(userLoginDto);
            if(result != null)
            {
                user = await _userService.GetByEmail(userLoginDto.Email);
                //user.Role = userLoginDto.Role;
            }
            string token = CreateToken(user);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            return Ok(token);
        }

        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmail(email);
            return StatusCode(StatusCodes.Status200OK, user);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(UserDto user)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, user);

            return Ok(token);
        }

        [HttpPost("logout"), Authorize]
        public IActionResult Logout()
        {
            try
            {
                // Xóa cookie refreshToken
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("refreshToken");
                Response.Headers.Add("Clear-Token", "true"); // Gửi header để thông báo việc xóa token từ phía máy khách


                return Ok("Logout successful");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, UserDto user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(UserDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
