using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Gymify.Web.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}