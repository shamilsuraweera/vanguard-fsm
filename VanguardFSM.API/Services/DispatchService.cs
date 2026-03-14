using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Data;
using VanguardFSM.Shared.Models;
using VanguardFSM.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using VanguardFSM.API.Hubs;

namespace VanguardFSM.API.Services;

public class DispatchService : IDispatchService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<DispatchService> _logger;

    public DispatchService(AppDbContext context, IHubContext<NotificationHub> hubContext, ILogger<DispatchService> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, ServiceStatus newStatus)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        task.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignTaskAsync(int taskId, int workerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            var worker = await _context.Users.FindAsync(workerId);
            if (worker == null) return false;

            task.Status = ServiceStatus.Dispatched;
            task.AssignedWorkerId = workerId;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Broadcast after commit
            try
            {
                await _hubContext.Clients.All.SendAsync("NewTaskAssigned", new { WorkerId = workerId, Title = task.Title });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast task assignment for task {TaskId}", taskId);
                // Note: Task is assigned, but broadcast failed. In production, retry or queue.
            }

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> AcceptTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.Status = ServiceStatus.InProgress;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<User>> SuggestWorkersAsync(int taskId, double radiusMeters)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task?.Location == null)
            return Enumerable.Empty<User>();

        return await _context.Users
            .Where(u => u.IsAvailable && u.LastKnownLocation != null
                && u.LastKnownLocation.IsWithinDistance(task.Location, radiusMeters))
            .ToListAsync();
    }

    public async Task<bool> UpdateWorkOrderAsync(int taskId, ServiceUpdateModel update)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.Description += $"\n\n--- Work Log ({DateTime.Now:g}) ---\nNotes: {update.Notes}\nParts: {update.PartsUsed}";
        task.Status = ServiceStatus.Completed;

        await _context.SaveChangesAsync();

        try
        {
            await _hubContext.Clients.All.SendAsync("TaskCompleted", taskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast task completion for task {TaskId}", taskId);
        }

        return true;
    }
}