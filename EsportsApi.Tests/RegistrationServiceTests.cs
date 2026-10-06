using Xunit;
using EsportsAPI.DTOs;
using EsportsAPI.Data;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace EsportsApi.Tests;

public class RegistrationServiceTests
{
    [Fact]
    public async Task RegisterTeam_TournamentDoesNotExist_ReturnsTournamentNotFound()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        using var context = new EsportsDbContext(options);

        var service = new RegistrationService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, Status = TournamentStatus.Registration, StartDate = DateTime.UtcNow });
        context.SaveChanges();

        //Act
        var result = await service.RegisterTeam(2, 1);

        //Assert
        Assert.Equal(RegistrationResultStatus.TournamentNotFound, result.Status);
    }

    [Fact]
    public async Task RegisterTeam_TeamAlreadyRegistered_ReturnsAlreadyRegistered()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        using var context = new EsportsDbContext(options);
        var service = new RegistrationService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, Status = TournamentStatus.Registration, StartDate = DateTime.UtcNow });
        context.Registrations.Add( new Registration { TournamentId = 1, TeamId = 1, RegisteredAt = DateTime.UtcNow});
        context.SaveChanges();

        //Act
        var result = await service.RegisterTeam(1, 1);

        //Assert
        Assert.Equal(RegistrationResultStatus.AlreadyRegistered, result.Status);
    }

    [Fact]
    public async Task RegisterTeam_TournamentAtCapacity_ReturnsTournamentFull()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        using var context = new EsportsDbContext(options);
        var service = new RegistrationService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL"});
        context.Teams.Add( new Team { Id = 2, Name = "Team SoloMid", Tag = "TSM"});
        context.Teams.Add( new Team { Id = 3, Name = "DragonX", Tag = "DRX"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 2, Status = TournamentStatus.Registration, StartDate = DateTime.UtcNow });
        context.Registrations.Add( new Registration {TeamId = 1, TournamentId = 1, RegisteredAt = DateTime.UtcNow});
        context.Registrations.Add( new Registration {TeamId = 2, TournamentId = 1, RegisteredAt = DateTime.UtcNow});
        context.SaveChanges();

        //Act
        var result = await service.RegisterTeam(1, 3);

        //Assert
        Assert.Equal(RegistrationResultStatus.TournamentFull, result.Status);
    }

    [Fact]
    public async Task RegisterTeam_TournamentPastRegistration_ReturnsTournamentNotOpen()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<EsportsDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        using var context = new EsportsDbContext(options);
        var service = new RegistrationService(context);

        context.Teams.Add( new Team { Id = 1, Name = "Team Liquid", Tag = "TL"});
        context.Teams.Add( new Team { Id = 2, Name = "Team SoloMid", Tag = "TSM"});
        context.Tournaments.Add( new Tournament { Id = 1, Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 2, Status = TournamentStatus.Locked, StartDate = DateTime.UtcNow });
        context.SaveChanges();

        //Act
        var result = await service.RegisterTeam(1, 1);

        //Assert
        Assert.Equal(RegistrationResultStatus.TournamentNotOpen, result.Status);
    }
}