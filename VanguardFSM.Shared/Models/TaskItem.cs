using VanguardFSM.Shared.Enums;
using NetTopologySuite.Geometries;

namespace VanguardFSM.Shared.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // The current stage in the 8-stage pipeline
    public ServiceStatus Status { get; set; } = ServiceStatus.Incoming;
    
    public Point? Location { get; set; }
}