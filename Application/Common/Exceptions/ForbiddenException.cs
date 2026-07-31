using Application.Common.Models;
namespace Application.Common
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden.", string errorCode = "FORBIDDEN")
            : base(message, errorCode, 403, null) { }
    }
}
