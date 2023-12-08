using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.ServiceType
{
    public class ServiceTypeCreateDto
    {
        public Guid ServiceTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
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
