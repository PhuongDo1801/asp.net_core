    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Exceptions
{
    public class NotFoundException : Exception
    {
        public List<string> ErrorMsgs { get; set; }

        public NotFoundException(string msg) : base(msg)
        {

        }

        public NotFoundException(List<string> errorMsgs)
        {
            ErrorMsgs = errorMsgs;
        }
    }
}
