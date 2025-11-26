using Microsoft.AspNetCore.SignalR;

namespace Gymify.Web.Services;

public class CustomUserIdProviderService : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // SignalR шукатиме claim з назвою "UserProfileId"
        return connection.User?.FindFirst("UserProfileId")?.Value;
    }
}
