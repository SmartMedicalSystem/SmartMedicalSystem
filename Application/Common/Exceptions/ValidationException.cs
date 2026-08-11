using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Common
{
    public class ValidationException : AppException
    {
        public ValidationException(string message = "Validation failed.", IEnumerable<ErrorDetail>? errors = null)
            : base(message, "VALIDATION_ERROR", 400, errors) { }
    }
}
