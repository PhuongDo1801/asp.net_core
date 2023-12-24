﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.User
{
    public class UserUpdateDto
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Tên người dùng
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
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
        /// DoB
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// ngày sửa
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
        /// <summary>
        /// người sửa
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
