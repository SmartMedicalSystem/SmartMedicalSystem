using Application.Common.Models;
namespace Application.Common
{
    public class ConflictException : AppException
    {
        public ConflictException(string message, string errorCode = "CONFLICT")
            : base(message, errorCode, 409, null) { }
    }
}
