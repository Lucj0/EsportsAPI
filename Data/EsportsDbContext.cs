using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Data;

public class EsportsDbContext : DbContext
{
    public EsportsDbContext(DbContextOptions<EsportsDbContext> options) : base(options)
    {
        
    }
}