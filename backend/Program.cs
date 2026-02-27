using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using WikiVideojuegos.Data;
using WikiVideojuegos.DTOs;
using WikiVideojuegos.Models;

var builder = WebApplication.CreateBuilder(args);

// SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not set");
var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1) // 1 min de tolerancia por desfase de reloj
        };
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                ctx.Response.StatusCode = 401;
                return ctx.Response.WriteAsJsonAsync(new { message = "Token inválido o expirado." });
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Wiki Videojuegos API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT. Ejemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wiki Videojuegos API v1");
    c.RoutePrefix = string.Empty; // Swagger en la raíz: http://localhost:5000/
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Email = "admin@wiki.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Name = "Administrador",
            Role = "admin"
        });
        db.Users.Add(new User
        {
            Email = "usuario@wiki.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("usuario123"),
            Name = "Usuario Prueba",
            Role = "user"
        });
        db.SaveChanges();
    }
    var defaultGames = new[]
    {
        new Game { Name = "The Legend of Zelda: Breath of the Wild", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lbf.png", Year = 2017, Description = "Aventura de mundo abierto de Nintendo. Explora Hyrule, resuelve santuarios y enfréntate a Ganon." },
        new Game { Name = "Elden Ring", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co4j1z.png", Year = 2022, Description = "RPG de acción de FromSoftware con mundo abierto. Historia de George R. R. Martin y gameplay tipo Souls." },
        new Game { Name = "Red Dead Redemption 2", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lbd.png", Year = 2018, Description = "Western de Rockstar. Vive la historia de Arthur Morgan y la banda de Dutch en el Lejano Oeste." },
        new Game { Name = "God of War Ragnarök", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co6boo.png", Year = 2022, Description = "Secuela de God of War (2018). Kratos y Atreus se enfrentan al Ragnarök en los Nueve Reinos." },
        new Game { Name = "Hollow Knight", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1xzm.png", Year = 2017, Description = "Metroidvania indie. Explora Hallownest, mejora habilidades y descubre secretos en un mundo de insectos." },
        new Game { Name = "Celeste", ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lbr.png", Year = 2018, Description = "Plataformas 2D de precisión. Madeline escala la montaña Celeste en una historia sobre ansiedad y superación." }
    };
    foreach (var game in defaultGames)
    {
        if (!db.Games.Any(g => g.Name == game.Name))
        {
            db.Games.Add(game);
        }
    }
    db.SaveChanges();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// --- Auth ---
app.MapPost("/api/auth/register", async (RegisterRequest req, AppDbContext db) =>
{
    if (await db.Users.AnyAsync(u => u.Email == req.Email))
        return Results.BadRequest(new { message = "El email ya está registrado." });
    var user = new User
    {
        Email = req.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
        Name = req.Name,
        Role = "user"
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    var token = JwtHelper.GenerateToken(user, app.Configuration);
    return Results.Ok(new AuthResponse(token, new UserDto(user.Id, user.Email, user.Name, user.Role)));
});

app.MapPost("/api/auth/login", async (LoginRequest req, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();
    var token = JwtHelper.GenerateToken(user!, app.Configuration);
    return Results.Ok(new AuthResponse(token, new UserDto(user.Id, user.Email, user.Name, user.Role)));
});

app.MapGet("/api/auth/me", async (ClaimsPrincipal user, AppDbContext db) =>
{
    var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var id))
        return Results.Unauthorized();
    var u = await db.Users.FindAsync(id);
    if (u == null) return Results.NotFound();
    return Results.Ok(new UserDto(u.Id, u.Email, u.Name, u.Role));
}).RequireAuthorization();

// --- Games (public read, admin write) ---
app.MapGet("/api/games", async (AppDbContext db) =>
{
    var list = await db.Games.OrderBy(g => g.Name).ToListAsync();
    return Results.Ok(list.Select(g => new GameDto(g.Id, g.Name, g.ImageUrl, g.Year, g.Description, g.CreatedAt)));
});

app.MapGet("/api/games/{id:int}", async (int id, AppDbContext db) =>
{
    var g = await db.Games.FindAsync(id);
    if (g == null) return Results.NotFound();
    return Results.Ok(new GameDto(g.Id, g.Name, g.ImageUrl, g.Year, g.Description, g.CreatedAt));
});

app.MapPost("/api/games", async (CreateGameRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    var game = new Game { Name = req.Name, ImageUrl = req.ImageUrl, Year = req.Year, Description = req.Description };
    db.Games.Add(game);
    await db.SaveChangesAsync();
    return Results.Created($"/api/games/{game.Id}", new GameDto(game.Id, game.Name, game.ImageUrl, game.Year, game.Description, game.CreatedAt));
}).RequireAuthorization();

app.MapPut("/api/games/{id:int}", async (int id, UpdateGameRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    var game = await db.Games.FindAsync(id);
    if (game == null) return Results.NotFound();
    game.Name = req.Name; game.ImageUrl = req.ImageUrl; game.Year = req.Year; game.Description = req.Description;
    await db.SaveChangesAsync();
    return Results.Ok(new GameDto(game.Id, game.Name, game.ImageUrl, game.Year, game.Description, game.CreatedAt));
}).RequireAuthorization();

app.MapDelete("/api/games/{id:int}", async (int id, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    var game = await db.Games.FindAsync(id);
    if (game == null) return Results.NotFound();
    db.Games.Remove(game);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// --- Characters (public read, admin write) ---
app.MapGet("/api/characters", async (int? gameId, AppDbContext db) =>
{
    var query = db.Characters.Include(c => c.Game).AsQueryable();
    if (gameId.HasValue) query = query.Where(c => c.GameId == gameId.Value);
    var list = await query.OrderBy(c => c.Name).ToListAsync();
    return Results.Ok(list.Select(c => new CharacterDetailDto(c.Id, c.Name, c.GameId, c.Game.Name, c.ImageUrl, c.Description, c.CreatedAt)));
});

app.MapGet("/api/characters/{id:int}", async (int id, AppDbContext db) =>
{
    var c = await db.Characters.Include(x => x.Game).FirstOrDefaultAsync(x => x.Id == id);
    if (c == null) return Results.NotFound();
    return Results.Ok(new CharacterDetailDto(c.Id, c.Name, c.GameId, c.Game.Name, c.ImageUrl, c.Description, c.CreatedAt));
});

app.MapPost("/api/characters", async (CreateCharacterRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    if (await db.Games.FindAsync(req.GameId) == null) return Results.BadRequest(new { message = "Juego no encontrado." });
    var ch = new Character { Name = req.Name, GameId = req.GameId, ImageUrl = req.ImageUrl, Description = req.Description };
    db.Characters.Add(ch);
    await db.SaveChangesAsync();
    var game = await db.Games.FindAsync(req.GameId);
    return Results.Created($"/api/characters/{ch.Id}", new CharacterDetailDto(ch.Id, ch.Name, ch.GameId, game!.Name, ch.ImageUrl, ch.Description, ch.CreatedAt));
}).RequireAuthorization();

app.MapPut("/api/characters/{id:int}", async (int id, UpdateCharacterRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    if (await db.Games.FindAsync(req.GameId) == null) return Results.BadRequest(new { message = "Juego no encontrado." });
    var ch = await db.Characters.FindAsync(id);
    if (ch == null) return Results.NotFound();
    ch.Name = req.Name; ch.GameId = req.GameId; ch.ImageUrl = req.ImageUrl; ch.Description = req.Description;
    await db.SaveChangesAsync();
    var game = await db.Games.FindAsync(req.GameId);
    return Results.Ok(new CharacterDetailDto(ch.Id, ch.Name, ch.GameId, game!.Name, ch.ImageUrl, ch.Description, ch.CreatedAt));
}).RequireAuthorization();

app.MapDelete("/api/characters/{id:int}", async (int id, ClaimsPrincipal user, AppDbContext db) =>
{
    if (!await IsAdmin(user, db)) return Results.Forbid();
    var ch = await db.Characters.FindAsync(id);
    if (ch == null) return Results.NotFound();
    db.Characters.Remove(ch);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();

static async Task<bool> IsAdmin(ClaimsPrincipal user, AppDbContext db)
{
    var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var id)) return false;
    var u = await db.Users.FindAsync(id);
    return u?.Role == "admin";
}

file static class JwtHelper
{
    public static string GenerateToken(User user, IConfiguration config)
    {
        var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret no configurado.");
        if (secret.Length < 32)
            throw new InvalidOperationException("Jwt:Secret debe tener al menos 32 caracteres.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expMinutes = double.Parse(config["Jwt:ExpirationMinutes"] ?? "60");
        var exp = DateTime.UtcNow.AddMinutes(expMinutes);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: exp,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>Clase expuesta para permitir tests con WebApplicationFactory.</summary>
public partial class Program { }
