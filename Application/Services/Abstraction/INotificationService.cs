using Domain.Enums;

namespace Domain.IRepository
{
    public interface INotificationService
    {
        Task BroadcastAsync(
            string title,
            string message);

        Task SendToRoleAsync(
            string role,
            string title,
            string message);

        Task SendToUserAsync(
            int userId,
            string message,
            NotificationType type,
            int? requestLabsId = null,
            int? patientResultId = null);
    }
}