namespace Application.Services.Abstraction
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? BasePersonId { get; }
    }
}