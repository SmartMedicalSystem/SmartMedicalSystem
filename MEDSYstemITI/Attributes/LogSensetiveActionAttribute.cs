using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Attributes;

public class LogSensitiveActionAttribute : TypeFilterAttribute
{
    public LogSensitiveActionAttribute()
        : base(typeof(LogSensitiveActionFilter))
    {
    }
}