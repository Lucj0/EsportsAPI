using Xunit;
using EsportsAPI.Controllers;
using EsportsAPI.Data;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using EsportsAPI.DTOs;
using System.Net.Http.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EsportsApi.Tests;

public class EsportsIntegrationTests : IClassFixture<EsportsApiFactory>
{
    private readonly HttpClient _client;

    public EsportsIntegrationTests(EsportsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTournament_WithoutToken_ReturnsNotAuthorized()
    {
        //Arrange
        var incomingTournament = new CreateTournamentDto { Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, StartDate = DateTime.UtcNow };

        //Act
        var response = await _client.PostAsJsonAsync("/tournament", incomingTournament);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTournament_WithInvalidRole_ReturnsForbidden()
    {
        //Arrange
        var incomingTournament = new CreateTournamentDto { Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, StartDate = DateTime.UtcNow };
        var registerDto = new RegisterDto { Username = "participant-user", Password = "ParticipantPassword" };
        var loginDto = new LoginDto { Username = "participant-user", Password = "ParticipantPassword" };

        await _client.PostAsJsonAsync("/auth/register", registerDto);
        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);

        //read token out of the response
        var token = await response.Content.ReadAsStringAsync();

        //attach the token to client (resets for every test)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var result = await _client.PostAsJsonAsync("/tournament", incomingTournament);

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task CreateTournament_WithValidRole_ReturnsSuccess()
    {
        //Arrange
        var incomingTournament = new CreateTournamentDto { Name = "Summer Showdown", GameTitle = "League of Legends", MaxTeams = 4, StartDate = DateTime.UtcNow };
        var registerDto = new RegisterDto { Username = "organizer-user", Password = "OrganizerPassword" };
        var loginDto = new LoginDto { Username = "organizer-user", Password = "OrganizerPassword" };
        var promoteDto = new PromoteDto { Key = "test-organizer-key"};

        await _client.PostAsJsonAsync("/auth/register", registerDto);
        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var token = await response.Content.ReadAsStringAsync();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/auth/promote", promoteDto);

        // Stateless JWT means promote didn't automatically apply new role to user, so have to login again to recieve
        // token with Organizer Role claim
        response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        token = await response.Content.ReadAsStringAsync();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var result = await _client.PostAsJsonAsync("/tournament", incomingTournament);

        //Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);

        //registration has Json enum converter option that test container does not copy over so 
        //I have to add the option myself to the deserializer. If not, deserializer doesn't know enum gets converted to a string
        //so it tries to read Json with default options and expects enum to be a number  which it isnt. 
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        var returnedTournament = await result.Content.ReadFromJsonAsync<TournamentDto>(options);
        Assert.NotNull(returnedTournament);
        Assert.Equal("Summer Showdown", returnedTournament.Name);
    }
}