using Microsoft.EntityFrameworkCore;
using VanguardFSM.Shared.Models; // Accessing our shared models

namespace VanguardFSM.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // This represents your table in SQL Server
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}