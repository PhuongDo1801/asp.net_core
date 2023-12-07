using AutoMapper;
using MyAspnetCore.DTO.User;
using MyAspnetCore.Entities;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyAspnetCore.Services
{
    public class UserService : BaseService<User, UserDto, UserCreateDto, UserUpdateDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
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

        public async Task<bool> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.Login(userLoginDto.Email, userLoginDto.Password);
            if (user == null)
            {
                throw new NotFoundException(new List<string> { ResourceVN.NotFoundAcc });
            }
            // bool isPasswordCorrect = string.Equals(userLoginDto.Password, user.Password, StringComparison.Ordinal);

            return true;
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
    }
}
