using AutoMapper;
using MyAspnetCore.DTO.User;
using MyAspnetCore.Entities;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
