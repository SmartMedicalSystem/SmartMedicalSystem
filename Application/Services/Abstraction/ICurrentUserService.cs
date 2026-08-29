namespace Application.Services.Abstraction
{
    /// <summary>
    /// Provides access to the authenticated HTTP user's identity and permissions.
    /// Implemented by Infrastructure so Application contracts do not depend on ASP.NET infrastructure.
    /// </summary>
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? BasePersonId { get; }
        bool HasPermission(string permission);
    }
}
