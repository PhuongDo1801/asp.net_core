using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.Provider
{
    public class ProviderUpdateDto
    {
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string Description { get; set; }
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
