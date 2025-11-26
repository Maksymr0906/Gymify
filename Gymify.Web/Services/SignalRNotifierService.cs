using Gymify.Application.Services.Interfaces;
using Gymify.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Gymify.Web.Services
{
    public class SignalRNotifierService : INotifierService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotifierService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushAsync(Guid userId, string method, object data)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync(method, data);
        }
    }
}