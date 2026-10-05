using EsportsAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Data;

public class EsportsDbContext : DbContext
{
    public EsportsDbContext(DbContextOptions<EsportsDbContext> options) : base(options)
    {
    }

    public DbSet<Tournament> Tournaments { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Registration> Registrations { get; set; }

    public DbSet<User> Users { get; set; }
}