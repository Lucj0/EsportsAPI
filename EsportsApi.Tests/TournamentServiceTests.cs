using Xunit;
using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EsportsApi.Tests;

public class TournamentServiceTests
{
    [Fact]
    public async Task Lock_OddNumberOfTeams_ReturnsOddTeamCount()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        using var context = new EsportsDbContext(options);
        var service = new TournamentService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, Status = TournamentStatus.Registration, StartDate = DateTime.UtcNow });
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 1, RegisteredAt = DateTime.UtcNow});
        context.SaveChanges();

        //Act
        var result = await service.Lock(1);

        //Assert
        Assert.Equal(TransitionResultStatus.OddTeamCount, result);
    }

    [Fact]
    public async Task Lock_EvenTeams_ReturnsSucessWithRandomSeeding()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        using var context = new EsportsDbContext(options);
        var service = new TournamentService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL" });
        context.Teams.Add( new Team { Id = 2, Name = "Team SoloMid", Tag = "TSM"});
        context.Teams.Add( new Team { Id = 3, Name = "Cloud9 Esports", Tag = "C9"});
        context.Teams.Add( new Team { Id = 4, Name = "Sk Telecom1", Tag = "T1"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, Status = TournamentStatus.Registration, StartDate = DateTime.UtcNow });
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 1, RegisteredAt = DateTime.UtcNow});
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 2, RegisteredAt = DateTime.UtcNow});
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 3, RegisteredAt = DateTime.UtcNow});
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 4, RegisteredAt = DateTime.UtcNow});
        context.SaveChanges();

        //Act
        var result = await service.Lock(1);

        //Assert
        Assert.Equal(TransitionResultStatus.Success, result);

        var seedings = await context.Registrations
            .Where(r => r.TournamentId == 1)
            .Select(r => r.Seed)
            .OrderBy(s => s)
            .ToListAsync();

        Assert.Equal(new int?[] {1, 2, 3, 4}, seedings);
    }
}