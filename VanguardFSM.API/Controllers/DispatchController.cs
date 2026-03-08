using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Data;
using VanguardFSM.Shared.Models;
using NetTopologySuite.Geometries;

namespace VanguardFSM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispatchController : ControllerBase
{
    private readonly AppDbContext _context;

    public DispatchController(AppDbContext context)
    {
        _context = context;
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
}