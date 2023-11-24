using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Exceptions
{
    public class ValidateException:Exception
    {
        public Dictionary<string, List<string>> ErrorMsgs { get; set; }
        public ValidateException(string message) : base(message)
        {

        }

        public ValidateException(Dictionary<string, List<string>> errorMsgs)
        {
            ErrorMsgs = errorMsgs;
        }
    }
}
