using System.Net;
using System.Net.Http.Json;
using WikiVideojuegos.DTOs;
using Xunit;

namespace WikiVideojuegos.Tests;

public class AuthTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public AuthTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ConCredencialesCorrectas_DevuelveTokenYUsuario()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@wiki.com", "admin123"));
        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.NotEmpty(auth.Token);
        Assert.Equal("admin@wiki.com", auth.User.Email);
        Assert.Equal("admin", auth.User.Role);
    }

    [Fact]
    public async Task Login_ConPasswordIncorrecta_Devuelve401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@wiki.com", "wrong"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ConEmailInexistente_Devuelve401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("noexiste@wiki.com", "admin123"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_NuevoUsuario_DevuelveTokenYUsuario()
    {
        var email = $"test{Guid.NewGuid():N}@test.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "pass123", "Test User"));
        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.NotEmpty(auth.Token);
        Assert.Equal(email, auth.User.Email);
        Assert.Equal("user", auth.User.Role);
    }

    [Fact]
    public async Task Register_EmailRepetido_Devuelve400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("admin@wiki.com", "pass123", "Otro"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AuthMe_SinToken_Devuelve401()
    {
        var response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthMe_ConTokenValido_DevuelveUsuario()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@wiki.com", "admin123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
        var response = await _client.GetAsync("/api/auth/me");
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("admin@wiki.com", user.Email);
    }
}
