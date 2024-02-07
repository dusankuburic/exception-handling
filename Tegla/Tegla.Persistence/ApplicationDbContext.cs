using Microsoft.EntityFrameworkCore;
using Tegla.Domain.Models.Items;

namespace Tegla.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Item> Items { get; set; }
}
