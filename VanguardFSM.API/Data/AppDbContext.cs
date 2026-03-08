using Microsoft.EntityFrameworkCore;
using VanguardFSM.Shared.Models; // Accessing our shared models
using VanguardFSM.Shared.Enums;

namespace VanguardFSM.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    // This represents your table in SQL Server
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<User> Users { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)

{
    // 1. Seed the Task (London - Westminster)
    modelBuilder.Entity<TaskItem>().HasData(
        new TaskItem { 
            Id = 1, 
            Title = "Repair AC - Customer A", 
            Description = "Standard maintenance.",
            Status = VanguardFSM.Shared.Enums.ServiceStatus.Incoming,
            Location = new NetTopologySuite.Geometries.Point(-0.1278, 51.5074) { SRID = 4326 } 
        }
    );

    // 2. Seed Workers
    modelBuilder.Entity<User>().HasData(
        new User { 
            Id = 1, 
            Name = "Close Worker (1km away)", 
            Role = "Worker", 
            IsAvailable = true,
            // Located very near the task
            LastKnownLocation = new NetTopologySuite.Geometries.Point(-0.1400, 51.5000) { SRID = 4326 } 
        },
        new User { 
            Id = 2, 
            Name = "Far Worker (20km away)", 
            Role = "Worker", 
            IsAvailable = true,
            // Located further away (near Heathrow)
            LastKnownLocation = new NetTopologySuite.Geometries.Point(-0.4543, 51.4700) { SRID = 4326 } 
        }
    );
}

}
