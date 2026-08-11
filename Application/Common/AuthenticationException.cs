namespace Application.Common
{
    /// <summary>Legacy authentication exception kept for backward compatibility.
    /// Prefer throwing UnauthorizedException for new code.</summary>
    public class AuthenticationException : UnauthorizedException
    {
        public AuthenticationException(string message, string errorCode = "INVALID_CREDENTIALS") : base(message, errorCode) { }
    }
}
