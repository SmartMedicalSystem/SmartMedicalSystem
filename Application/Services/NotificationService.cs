using Domain.Entities;
using Domain.IRepository;
using MEDSYstemITI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MEDSYstemITI.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IUnitOfWork _uow;

        public NotificationService(
            IHubContext<NotificationHub> hub,
            IUnitOfWork uow)
        {
            _hub = hub;
            _uow = uow;
        }

        public async Task BroadcastAsync(string title, string message)
        {
            await _hub.Clients.All.SendAsync(
                "ReceiveNotification",
                title,
                message);
        }

        public async Task SendToRoleAsync(string role, string title, string message)
        {
            await _hub.Clients.Group(role).SendAsync(
                "ReceiveNotification",
                title,
                message);
        }

        public async Task SendToUserAsync(int userId, string title, string message)
        {
            var notification = new Notification(
                userId,
                message,
                DateTime.UtcNow);

            await _uow.Notifications.AddAsync(notification);
            await _uow.SaveChangesAsync();

            await _hub.Clients.User(userId.ToString())
                .SendAsync(
                    "ReceiveNotification",
                    title,
                    message);
        }
    }
}
