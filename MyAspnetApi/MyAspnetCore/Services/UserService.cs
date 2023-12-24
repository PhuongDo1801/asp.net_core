using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyAspnetCore.DTO.User;
using MyAspnetCore.Entities;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyAspnetCore.Services
{
    public class UserService : BaseService<User, UserDto, UserCreateDto, UserUpdateDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(int, IEnumerable<UserDto>)> GetListAsync(string? queryName, int? recordsPerPage, int? page)
        {
            IEnumerable<User> users = new List<User>();
            var totalRecord = 0;
            (totalRecord, users) = await _userRepository.GetListAsync(queryName, recordsPerPage, page);
            IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return (totalRecord, userDtos);
        }

        public static bool IsValidEmail(string email)
        {
            // Pattern kiểm tra định dạng email
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Sử dụng Regex để kiểm tra
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }

        public async Task<Dictionary<string, List<string>>> InsertErr(UserCreateDto userCreateDto)
        {
            Dictionary<string, List<string>> errorsList = new Dictionary<string, List<string>>();
            var isExistEmail = await _userRepository.IsExistEmail(userCreateDto.Email);
            if(isExistEmail)
            {
                errorsList.Add("Email", new List<string>() { string.Format(ResourceVN.EmailExist, userCreateDto.Email) });
            }
            var isValidEmail = IsValidEmail(userCreateDto.Email);
            if (userCreateDto.Email != null && userCreateDto.Email != "")
            {
                if (!IsValidEmail(userCreateDto.Email))
                {
                    errorsList.Add("Email", new List<string>() { ResourceVN.InvalidEmail });
                }

            }
            if (userCreateDto.DateOfBirth != null)
            {
                if (userCreateDto.DateOfBirth > DateTime.Now)
                {
                    errorsList.Add("DateOfBirth", new List<string>() { ResourceVN.InvalidDob });
                }
            }
            return errorsList;
        }

        public async Task<int> Register(UserCreateDto useCreateDto)
        {
            var passwordHash = await HashPassword(useCreateDto.Password);
            useCreateDto.Password = passwordHash;
            var errorsList = await InsertErr(useCreateDto);

            if (errorsList.Count > 0)
            {
                throw new ValidateException(errorsList);
            }
            var entity = _mapper.Map<User>(useCreateDto);
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                if (name == $"UserId")
                {
                    property.SetValue(entity, Guid.NewGuid());
                }
                else if (name == $"CreatedDate")
                {
                    property.SetValue(entity, DateTime.Now);
                }
                else if (name == $"CreatedBy")
                {
                    property.SetValue(entity, null);
                }
            }



            var result = await _userRepository.Register(entity);

            return result;
        }

        public async Task<UserDto> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.GetByEmail(userLoginDto.Email);
            if(user != null)
            {
                var verify = await VerifyPassword(userLoginDto.Password, user.Password);
                if (verify)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    return userDto;
                }
                else
                {
                    // Trường hợp mật khẩu không đúng
                    throw new UnauthorizedAccessException("Invalid password");
                }
            }
            else
            {
                throw new NotFoundException(new List<string> { ResourceVN.NotFoundAcc });
            }
            
            //var user = await _userRepository.Login(userLoginDto.Email, userLoginDto.Password);
            //if (user == null)
            //{
            //    throw new NotFoundException(new List<string> { ResourceVN.NotFoundAcc });
            //}
            //var userDto = _mapper.Map<UserDto>(user);
            //return userDto;
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
            {
                throw new NotFoundException(new List<string> { ResourceVN.Err_NotFound });
            }

            var entityDto = _mapper.Map<UserDto>(user);

            return entityDto;
        }

        public Task<UserDto> GetUserInfo()
        {
            UserDto userInfo = new UserDto(); // Tạo một đối tượng UserInfo (bạn cần định nghĩa class UserInfo)

            if (_httpContextAccessor.HttpContext != null)
            {
                var user = _httpContextAccessor.HttpContext.User;

                userInfo.UserName = user.FindFirstValue(ClaimTypes.Name);
                userInfo.Email = user.FindFirstValue(ClaimTypes.Email);
                userInfo.Role = user.FindFirstValue(ClaimTypes.Role);

                //Thêm các thông tin khác tùy thuộc vào cấu trúc claim trong hệ thống của bạn
                //userInfo.OtherInfo = user.FindFirstValue("OtherClaimType");
            }

            return Task.FromResult(userInfo);
        }

        public async Task<string> HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Tạo một MemoryStream từ mảng byte
                using (var stream = new MemoryStream(passwordBytes))
                {
                    // Băm mật khẩu sử dụng SHA-256
                    byte[] passwordHash = await sha256.ComputeHashAsync(stream);

                    // Chuyển đổi mảng byte thành chuỗi hex
                    string hashedPassword = BitConverter.ToString(passwordHash).Replace("-", "").ToLower();

                    return hashedPassword;
                }
            }
        }

        public async Task<bool> VerifyPassword(string password, string storedPasswordHash)
        {
            using (var sha256 = SHA256.Create())
            {
                // Chuyển đổi mật khẩu nhập vào thành mảng byte
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(password);

                // Tạo một MemoryStream từ mảng byte
                using (var stream = new MemoryStream(enteredPasswordBytes))
                {
                    // Băm mật khẩu nhập vào sử dụng SHA-256
                    byte[] enteredPasswordHash = await sha256.ComputeHashAsync(stream);

                    // Chuyển đổi mảng byte thành chuỗi hex
                    string hashedEnteredPassword = BitConverter.ToString(enteredPasswordHash).Replace("-", "").ToLower();

                    // So sánh giá trị băm mới với giá trị băm đã lưu trữ
                    return storedPasswordHash.Equals(hashedEnteredPassword);
                }
            }
        }
    }
}
