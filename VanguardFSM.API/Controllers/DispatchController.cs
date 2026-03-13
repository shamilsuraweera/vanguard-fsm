using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Data;
using VanguardFSM.Shared.Models;
using VanguardFSM.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using VanguardFSM.API.Hubs;

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

    // Functionality: Returns all tasks for the Kanban board
    [HttpGet("tasks")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        return await _context.Tasks.ToListAsync();
    }

    // Functionality: Updates status when a card is dragged in Kanban
    [HttpPut("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] ServiceStatus newStatus)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        task.Status = newStatus;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Functionality: Assigns a worker to a task and triggers a SignalR alert
    [HttpPost("assign-task/{taskId}/{workerId}")]
    public async Task<IActionResult> AssignTask(int taskId, int workerId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return NotFound();

        task.Status = ServiceStatus.Dispatched;
        // Development: In a real system, you would link the Task to the WorkerId here
        await _context.SaveChangesAsync();

        // Broadcast to SignalR Hub so the worker's dashboard refreshes
        await _hubContext.Clients.All.SendAsync("NewTaskAssigned", new { WorkerId = workerId, Title = task.Title });

        return Ok();
    }

    // Functionality: Called when a worker clicks "Accept Job"
    [HttpPost("accept-task/{taskId}")]
    public async Task<IActionResult> AcceptTask(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return NotFound();

        task.Status = ServiceStatus.InProgress;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Task 4.1: New Endpoint for Work Updates
    // Functionality: Receives notes/parts and marks the job as Completed.
    [HttpPost("update-work/{taskId}")]
    public async Task<IActionResult> UpdateWorkOrder(int taskId, [FromBody] ServiceUpdateModel update)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return NotFound();

        // Development: Appending the new report to the existing description
        task.Description += $"\n\n--- Work Log ({DateTime.Now:g}) ---\nNotes: {update.Notes}\nParts: {update.PartsUsed}";
        task.Status = ServiceStatus.Completed;

        await _context.SaveChangesAsync();
        
        // Notify Dispatcher via SignalR that the job is done
        await _hubContext.Clients.All.SendAsync("TaskCompleted", taskId);

        return Ok();
    }
}