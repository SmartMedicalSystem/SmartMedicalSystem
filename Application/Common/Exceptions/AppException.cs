using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Common
{
    public abstract class AppException : System.Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }
        public IReadOnlyCollection<ErrorDetail> Errors { get; }

        protected AppException(string message, string errorCode, int statusCode, IEnumerable<ErrorDetail>? errors = null)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Errors = errors is null ? new List<ErrorDetail>() : new List<ErrorDetail>(errors);
        }
    }
}
