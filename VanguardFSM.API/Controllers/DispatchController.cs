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
    private readonly IHubContext<NotificationHub> _hubContext;

    public DispatchController(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // 1. Fetch all tasks for the Dashboard table
    [HttpGet("tasks")]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return Ok(tasks);
    }

    // 2. Spatial Query: Find workers within a specific radius of a task
    [HttpGet("suggest-workers/{taskId}/{radiusInMeters}")]
    public async Task<IActionResult> GetSuggestedWorkers(int taskId, double radiusInMeters)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task?.Location == null) return NotFound("Task location not set.");

        var nearbyWorkers = await _context.Users
            .Where(u => u.Role == "Worker" && u.IsAvailable && u.LastKnownLocation != null)
            .Where(u => u.LastKnownLocation!.Distance(task.Location!) <= radiusInMeters) 
            .OrderBy(u => u.LastKnownLocation!.Distance(task.Location!))
            .ToListAsync();

        return Ok(nearbyWorkers);
    }

    // 3. Assign Task: Update status and notify worker via SignalR
    [HttpPost("assign-task/{taskId}/{workerId}")]
    public async Task<IActionResult> AssignTask(int taskId, int workerId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        var worker = await _context.Users.FindAsync(workerId);

        if (task == null || worker == null) return NotFound("Task or Worker not found.");

        task.Status = VanguardFSM.Shared.Enums.ServiceStatus.Dispatched;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("NewTaskAssigned", new {
            TaskTitle = task.Title,
            Description = task.Description,
            Location = task.Location
        });

        return Ok($"Task {taskId} successfully dispatched to {worker.Name}");
    }

    // 4. Health Check: Verify Database and API connectivity
    [HttpGet("db-check")]
    public async Task<IActionResult> CheckDb()
    {
        try 
        {
            bool canConnect = await _context.Database.CanConnectAsync();
            var taskCount = await _context.Tasks.CountAsync();
            
            return Ok(new { 
                Status = "Online", 
                TotalTasks = taskCount, 
                Timestamp = DateTime.Now.ToShortTimeString() 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "Offline", Error = ex.Message });
        }
    }
}