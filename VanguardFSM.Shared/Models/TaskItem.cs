using NetTopologySuite.Geometries;

namespace VanguardFSM.Shared.Models;

public class TaskItem 
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Point? Location { get; set; }
}