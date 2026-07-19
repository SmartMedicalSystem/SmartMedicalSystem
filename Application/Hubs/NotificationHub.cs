using Microsoft.AspNetCore.SignalR;

namespace MEDSYstemITI.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
