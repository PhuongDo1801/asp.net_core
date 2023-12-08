﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.Service
{
    public class ServiceCreateDto
    {
        public Guid ServiceId { get; set; }

        public Guid ProviderId { get; set; }
        public string? ProviderName { get; set; }

        public Guid ServiceTypeId { get; set; }
        public string? TypeName { get; set; }

        public string ServiceName { get; set; }

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
