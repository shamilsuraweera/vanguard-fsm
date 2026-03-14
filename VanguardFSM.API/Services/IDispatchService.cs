using VanguardFSM.Shared.Models;
using VanguardFSM.Shared.Enums;

namespace VanguardFSM.API.Services;

public interface IDispatchService
{
    Task<IEnumerable<TaskItem>> GetTasksAsync();
    Task<bool> UpdateStatusAsync(int id, ServiceStatus newStatus);
    Task<bool> AssignTaskAsync(int taskId, int workerId);
    Task<bool> AcceptTaskAsync(int taskId);
    Task<bool> UpdateWorkOrderAsync(int taskId, ServiceUpdateModel update);
    Task<IEnumerable<User>> SuggestWorkersAsync(int taskId, double radiusMeters);
}
