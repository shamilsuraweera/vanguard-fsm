using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Data;
using Microsoft.AspNetCore.SignalR;
using VanguardFSM.API.Hubs;
using VanguardFSM.Shared.Models;
using NetTopologySuite.Geometries;

namespace VanguardFSM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispatchController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext; // The SignalR engine

    public DispatchController(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

[HttpGet("suggest-workers/{taskId}/{radiusInMeters}")]
public async Task<IActionResult> GetSuggestedWorkers(int taskId, double radiusInMeters)
{
    // 1. Find the task's location
    var task = await _context.Tasks.FindAsync(taskId);
    if (task?.Location == null) return NotFound("Task location not set.");

    // 2. Query SQL Server for workers within the radius
    // This is where the 'Smart' part happens!
    var nearbyWorkers = await _context.Users
    .Where(u => u.Role == "Worker" && u.IsAvailable && u.LastKnownLocation != null) // Add null check
    .Where(u => u.LastKnownLocation!.Distance(task.Location!) <= radiusInMeters) // Use ! to tell compiler you checked it
    .OrderBy(u => u.LastKnownLocation!.Distance(task.Location!))
    .ToListAsync();

    return Ok(nearbyWorkers);
}
[HttpPost("assign-task/{taskId}/{workerId}")]
public async Task<IActionResult> AssignTask(int taskId, int workerId)
{
    var task = await _context.Tasks.FindAsync(taskId);
    var worker = await _context.Users.FindAsync(workerId);

    if (task == null || worker == null) return NotFound("Task or Worker not found.");

    // 1. Update the database status
    task.Status = VanguardFSM.Shared.Enums.ServiceStatus.Dispatched;
    await _context.SaveChangesAsync();

    // 2. Push the notification via SignalR
    // We send a message titled "NewTaskAssigned" directly to the worker
    await _hubContext.Clients.All.SendAsync("NewTaskAssigned", new {
        TaskTitle = task.Title,
        Description = task.Description,
        Location = task.Location
    });

    return Ok($"Task {taskId} successfully dispatched to {worker.Name}");
}
}