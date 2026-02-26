using System.Net;
using System.Net.Http.Json;
using WikiVideojuegos.DTOs;
using Xunit;

namespace WikiVideojuegos.Tests;

public class GamesTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public GamesTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetGames_SinAuth_DevuelveLista()
    {
        var response = await _client.GetAsync("/api/games");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task PostGame_SinAuth_Devuelve401()
    {
        var response = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest("Juego Test", null, 2024, "Desc"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostGame_ComoAdmin_CreaJuego()
    {
        await SetAdminToken();
        var response = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest("Zelda TOTK", "https://example.com/img.jpg", 2023, "Aventura"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var game = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(game);
        Assert.True(game.Id > 0);
        Assert.Equal("Zelda TOTK", game.Name);
        Assert.Equal(2023, game.Year);
    }

    [Fact]
    public async Task PutGame_ComoAdmin_Actualiza()
    {
        await SetAdminToken();
        var create = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest("Juego Original", null, 2020, null));
        var created = await create.Content.ReadFromJsonAsync<GameDto>();
        var response = await _client.PutAsJsonAsync($"/api/games/{created!.Id}", new UpdateGameRequest("Juego Actualizado", null, 2021, "Nueva desc"));
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.Equal("Juego Actualizado", updated!.Name);
        Assert.Equal(2021, updated.Year);
    }

    [Fact]
    public async Task DeleteGame_ComoAdmin_Borra()
    {
        await SetAdminToken();
        var create = await _client.PostAsJsonAsync("/api/games", new CreateGameRequest("Para Borrar", null, 2020, null));
        var created = await create.Content.ReadFromJsonAsync<GameDto>();
        var response = await _client.DeleteAsync($"/api/games/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var get = await _client.GetAsync($"/api/games/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    private async Task SetAdminToken()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@wiki.com", "admin123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
