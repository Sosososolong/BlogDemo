using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Helpers
{
    public class ResourceValidationError
    {
        //表示错误类型， 如：required，maxlength（angular显示需要错误类型）
        public string ValidatorKey { get; private set; }

        public string Message { get; set; }

        public ResourceValidationError(string message, string validatorKey = "")
        {
            ValidatorKey = validatorKey;
            Message = message;
        }
    }
}
