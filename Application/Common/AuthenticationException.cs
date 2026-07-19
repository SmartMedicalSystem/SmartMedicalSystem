namespace Application.Common
{
    /// <summary>Thrown by AuthService for login/registration failures (bad credentials, duplicate username/email, etc).</summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }
}
