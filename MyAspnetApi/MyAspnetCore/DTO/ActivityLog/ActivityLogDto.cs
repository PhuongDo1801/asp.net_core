using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.ActivityLog
{
    public class ActivityLogDto
    {
        public Guid ActivityLogId { get; set; }
        public Guid UserId { get; set; }
        public string InstanceId { get; set; }
        public string ServiceName { get; set; }
        public string ActivityDescription { get; set; }
        public string Result { get; set; }
        public DateTime CreatedDate { get; set; }
        //public string UserName { get; set; }
        //public string UserEmail { get; set; }
    }
}
