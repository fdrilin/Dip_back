using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

public class ResourceContext : DbContext
{
    public ResourceContext(DbContextOptions<ResourceContext> options)
        : base(options)
    {
    }

    public DbSet<ResourceItem> ResourceItems { get; set; } = null!;
}