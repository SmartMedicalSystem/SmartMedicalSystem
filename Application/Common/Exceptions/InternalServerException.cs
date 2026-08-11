using Application.Common.Models;
namespace Application.Common
{
    public class InternalServerException : AppException
    {
        public InternalServerException(string message = "An unexpected error occurred.", string errorCode = "INTERNAL_SERVER_ERROR")
            : base(message, errorCode, 500, null) { }
    }
}
