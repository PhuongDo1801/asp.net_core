using MyAspnetCore.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.User
{
    public class UserCreateDto
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Tên người dùng
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ResourceVN), ErrorMessageResourceName = nameof(ResourceVN.UserNameNotEmpty))]
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ResourceVN), ErrorMessageResourceName = nameof(ResourceVN.EmailNotEmpty))]
        public string Email { get; set; }
        /// <summary>
        /// SĐT
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// CCCD
        /// </summary>
        public string IdentityNumber { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ResourceVN), ErrorMessageResourceName = nameof(ResourceVN.PasswordNotEmpty))]
        public string Password { get; set; }
        /// <summary>
        /// DoB
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// tạo bơi
        /// </summary>
        public string? CreatedBy { get; set; }
    }
}
