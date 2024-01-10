using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class User : BaseEntity
    {
        #region
        public User()
        {
            
        }
        #endregion
        #region property
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public string AwsId { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        #endregion
    }
}
