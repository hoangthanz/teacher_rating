using Microsoft.AspNetCore.SignalR;

namespace teacher_rating.HubConfig;

public class RealtimeHub: Hub
{
    // public async Task Send(MessageSupport message)
    // {
    //     await Clients.All.SendAsync("Receive", message);
    // }
}