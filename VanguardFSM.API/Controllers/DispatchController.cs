using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Data;
using VanguardFSM.Shared.Models;
using VanguardFSM.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using VanguardFSM.API.Hubs;
using VanguardFSM.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace VanguardFSM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispatchController : ControllerBase
{
    private readonly DispatchService _dispatchService;
    private readonly ILogger<DispatchController> _logger;

    public DispatchController(DispatchService dispatchService, ILogger<DispatchController> logger)
    {
        _dispatchService = dispatchService;
        _logger = logger;
    }

    // Functionality: Returns all tasks for the Kanban board
    [HttpGet("tasks")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        try
        {
            var tasks = await _dispatchService.GetTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, "Internal server error");
        }
    }

    // Functionality: Updates status when a card is dragged in Kanban
    [HttpPut("update-status/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] ServiceStatus newStatus)
    {
        try
        {
            var result = await _dispatchService.UpdateStatusAsync(id, newStatus);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status for id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // Functionality: Assigns a worker to a task and triggers a SignalR alert
    [HttpPost("assign-task/{taskId}/{workerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTask(int taskId, int workerId)
    {
        try
        {
            var result = await _dispatchService.AssignTaskAsync(taskId, workerId);
            if (!result) return NotFound();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task {TaskId} to worker {WorkerId}", taskId, workerId);
            return StatusCode(500, "Internal server error");
        }
    }

    // Functionality: Called when a worker clicks "Accept Job"
    [HttpPost("accept-task/{taskId}")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> AcceptTask(int taskId)
    {
        try
        {
            var result = await _dispatchService.AcceptTaskAsync(taskId);
            if (!result) return NotFound();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting task {TaskId}", taskId);
            return StatusCode(500, "Internal server error");
        }
    }

    // Task 4.1: New Endpoint for Work Updates
    // Functionality: Receives notes/parts and marks the job as Completed.
    [HttpPost("update-work/{taskId}")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> UpdateWorkOrder(int taskId, [FromBody] ServiceUpdateModel update)
    {
        try
        {
            var result = await _dispatchService.UpdateWorkOrderAsync(taskId, update);
            if (!result) return NotFound();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work order for task {TaskId}", taskId);
            return StatusCode(500, "Internal server error");
        }
    }
}