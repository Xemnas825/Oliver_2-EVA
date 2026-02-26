using System.Net;
using System.Net.Http.Json;
using WikiVideojuegos.DTOs;
using Xunit;

namespace WikiVideojuegos.Tests;

public class CharactersTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public CharactersTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCharacters_SinAuth_DevuelveLista()
    {
        var response = await _client.GetAsync("/api/characters");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<CharacterDetailDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task PostCharacter_SinAuth_Devuelve401()
    {
        var gameId = await CreateGameAsAdmin();
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/characters", new CreateCharacterRequest("Link", gameId, null, "Hero"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostCharacter_ComoAdmin_CreaPersonaje()
    {
        var gameId = await CreateGameAsAdmin();
        var response = await _client.PostAsJsonAsync("/api/characters", new CreateCharacterRequest("Mario", gameId, null, "Fontanero"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var ch = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        Assert.NotNull(ch);
        Assert.True(ch.Id > 0);
        Assert.Equal("Mario", ch.Name);
        Assert.Equal(gameId, ch.GameId);
    }

    [Fact]
    public async Task GetCharacters_ConGameId_FiltraPorJuego()
    {
        var gameId = await CreateGameAsAdmin();
        await _client.PostAsJsonAsync("/api/characters", new CreateCharacterRequest("P1", gameId, null, null));
        var response = await _client.GetAsync($"/api/characters?gameId={gameId}");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<CharacterDetailDto>>();
        Assert.NotNull(list);
        Assert.All(list, c => Assert.Equal(gameId, c.GameId));
    }

    private async Task<int> CreateGameAsAdmin()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@wiki.com", "admin123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
        var create = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest("Super Mario", null, 1985, null));
        var game = await create.Content.ReadFromJsonAsync<GameDto>();
        return game!.Id;
    }
}
