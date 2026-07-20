using System.Collections.Generic;

namespace Application.Common
{
    /// <summary>Application-level exception that represents an entity not found condition.</summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.", "NOT_FOUND", 404) { }

        public NotFoundException(string message, string errorCode = "NOT_FOUND")
            : base(message, errorCode, 404) { }
    }
}
