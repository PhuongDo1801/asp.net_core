using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAspnetCore.Exceptions
{
    public class BaseException
    {
        #region Propperties

        public int ErrCode { get; set; }
        public string? DevMsg { get; set; }
        public string? UserMsg { get; set; }
        public string? TraceId { get; set; }
        public string? MoreInfo { get; set; }

        public Object? ErrorMsgs { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        #endregion
    }
}
