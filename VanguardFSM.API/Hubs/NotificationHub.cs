using Microsoft.AspNetCore.SignalR;

namespace VanguardFSM.API.Hubs;

// The Hub acts as the central traffic controller for real-time messages
public class NotificationHub : Hub
{
    // Workers will "Join" a group based on their Role or UserID
    public async Task JoinDispatchGroup(string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, role);
    }

    public override async Task OnConnectedAsync()
    {
        // Log connection for debugging (optional)
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
}