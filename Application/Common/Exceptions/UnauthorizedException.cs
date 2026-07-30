using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Common
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized.", string errorCode = "UNAUTHORIZED")
            : base(message, errorCode, 401, null) { }
    }
}
