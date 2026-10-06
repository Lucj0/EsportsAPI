using Xunit;
using EsportsAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Microsoft.Extensions.Configuration;

namespace EsportsApi.Tests;

public class EsportsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "k9Xw7m2PqL4vR8bN3zT6jY1sC5fV9gW2",
                ["Jwt:Issuer"] = "EsportsAPI",
                ["Jwt:Audience"] = "EsportsAPI",
                ["Auth:OrganizerKey"] = "test-organizer-key" 
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<EsportsDbContext>>();

            services.AddDbContext<EsportsDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));

           var provider = services.BuildServiceProvider();
           using var scope = provider.CreateScope();
           var context = scope.ServiceProvider.GetRequiredService<EsportsDbContext>();
           context.Database.EnsureCreated();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}