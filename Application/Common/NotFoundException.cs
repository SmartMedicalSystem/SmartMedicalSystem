using System;

namespace Application.Common
{
    /// <summary>Thrown by a service when a requested entity id does not exist (or is soft-deleted).</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.") { }
    }
}
