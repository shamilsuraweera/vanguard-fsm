using Microsoft.AspNetCore.SignalR;

namespace VanguardFSM.API.Hubs;

// Development: Inheriting from 'Hub' gives this class the ability to manage WebSocket connections, 
// group users, and broadcast messages to connected clients.
public class NotificationHub : Hub
{
    // Functionality: Allows specific types of users (e.g., "Technicians") to join a dedicated channel.
    // Development: Adds the current connection's unique ID to a named SignalR group.
    public async Task JoinDispatchGroup(string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, role);
    }

    // Development: An override method that fires automatically whenever a Blazor client successfully connects.
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[SignalR] Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    // Development: An override method that fires when a user closes their browser or loses connection.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[SignalR] Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    // Functionality: The core method that alerts the workforce that a new job has been assigned.
    // Development: This is called via HTTP/WebSocket from the Dispatcher's Dashboard. It then pushes 
    // an event named "NewTaskAssigned" to ALL connected clients simultaneously.
    public async Task SendTaskAssignment(int workerId, string taskTitle)
    {
        await Clients.All.SendAsync("NewTaskAssigned", new 
        { 
            WorkerId = workerId, 
            Title = taskTitle,
            Timestamp = DateTime.UtcNow 
        });
    }
}