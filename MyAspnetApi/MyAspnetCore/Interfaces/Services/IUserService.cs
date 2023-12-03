using MyAspnetCore.DTO.User;
using MyAspnetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IUserService : IBaseService<UserDto, UserCreateDto, UserUpdateDto>
    {
        public Task<(int, IEnumerable<UserDto>)> GetListAsync(string? queryName, int? recordsPerPage, int? page);

        public Task<int> Register(UserCreateDto userCreateDto);

        public Task<bool> Login(UserLoginDto userLoginDto);
    }
}
