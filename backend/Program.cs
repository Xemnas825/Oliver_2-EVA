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
        new Game { Name = "The Legend of Zelda: Breath of the Wild", ImageUrl = "https://i.blogs.es/15da49/zelda00/1366_2000.webp", Year = 2017, Description = "Aventura de mundo abierto de Nintendo. Explora Hyrule, resuelve santuarios y enfréntate a Ganon." },
        new Game { Name = "Elden Ring", ImageUrl = "https://i.blogs.es/c0b150/1024_2000/1366_2000.jpeg", Year = 2022, Description = "RPG de acción de FromSoftware con mundo abierto. Historia de George R. R. Martin y gameplay tipo Souls." },
        new Game { Name = "Red Dead Redemption 2", ImageUrl = "https://i.blogs.es/juegos/13424/red_dead_3__nombre_temporal_/fotos/maestras/red_dead_3__nombre_temporal_-4030936.jpg", Year = 2018, Description = "Western de Rockstar. Vive la historia de Arthur Morgan y la banda de Dutch en el Lejano Oeste." },
        new Game { Name = "God of War Ragnarök", ImageUrl = "https://static.wikia.nocookie.net/godofwar/images/c/ca/Portada_God_of_War_Ragnarok.png/revision/latest?cb=20211008000423&path-prefix=es", Year = 2022, Description = "Secuela de God of War (2018). Kratos y Atreus se enfrentan al Ragnarök en los Nueve Reinos." },
        new Game { Name = "Hollow Knight", ImageUrl = "https://i.3djuegos.com/juegos/11596/hollow_knight/fotos/ficha/hollow_knight-3915488.webp", Year = 2017, Description = "Metroidvania indie. Explora Hallownest, mejora habilidades y descubre secretos en un mundo de insectos." },
        new Game { Name = "Celeste", ImageUrl = "https://i.3djuegos.com/juegos/14243/celeste/fotos/ficha/celeste-3938712.webp", Year = 2018, Description = "Plataformas 2D de precisión. Madeline escala la montaña Celeste en una historia sobre ansiedad y superación." }
    };
    foreach (var game in defaultGames)
    {
        var existing = db.Games.FirstOrDefault(g => g.Name == game.Name);
        if (existing == null)
        {
            db.Games.Add(game);
        }
        else
        {
            existing.ImageUrl = game.ImageUrl;
            existing.Year = game.Year;
            existing.Description = game.Description;
        }
    }
    db.SaveChanges();

    // Seed personajes: insertar si no existen, o actualizar imagen/descripción si ya existen (así los cambios en las URLs se aplican al reiniciar)
    var zelda = db.Games.FirstOrDefault(g => g.Name.Contains("Zelda"));
    var elden = db.Games.FirstOrDefault(g => g.Name.Contains("Elden"));
    var rdr2 = db.Games.FirstOrDefault(g => g.Name.Contains("Red Dead"));
    var gow = db.Games.FirstOrDefault(g => g.Name.Contains("God of War"));
    var hollow = db.Games.FirstOrDefault(g => g.Name.Contains("Hollow Knight"));
    var celeste = db.Games.FirstOrDefault(g => g.Name.Contains("Celeste"));

    var defaultCharacters = new List<Character>();
    if (zelda != null)
    {
        defaultCharacters.Add(new Character { Name = "Link", GameId = zelda.Id, ImageUrl = "https://www.universozelda.com/wiki/images/f/f6/Artwork_Link_adulto_OoT.png", Description = "El héroe de Hyrule. Portador de la Espada Maestra, despierta tras un sueño de cien años para enfrentarse a Ganon y salvar a la princesa Zelda." });
        defaultCharacters.Add(new Character { Name = "Zelda", GameId = zelda.Id, ImageUrl = "https://static.wikia.nocookie.net/zelda/images/3/3d/TotK_-_Zelda_artwork_oficial.jpeg/revision/latest?cb=20230220230004&path-prefix=es", Description = "Princesa de Hyrule y guardiana del poder sagrado. Mantiene sellado a Ganon en el castillo mientras espera a Link." });
    }
    if (elden != null)
        defaultCharacters.Add(new Character { Name = "Melina", GameId = elden.Id, ImageUrl = "https://i.pinimg.com/736x/a6/49/42/a649426494b679afb3050d9d779d68a7.jpg", Description = "Misteriosa doncella que ofrece al Sinluz un acuerdo: guiarle al Árbol Áureo a cambio de que la lleve hasta la llama del Destino." });
    if (rdr2 != null)
        defaultCharacters.Add(new Character { Name = "Arthur Morgan", GameId = rdr2.Id, ImageUrl = "https://media.rockstargames.com/rockstargames-newsite/uploads/bcb6fd3bd0fbc7092e9d1973bb736f2c7ae05a7d.jpg", Description = "Protagonista y pistolero de la banda de Dutch van der Linde. Hombre leal que cuestiona el rumbo de la banda en el Lejano Oeste." });
    if (gow != null)
    {
        defaultCharacters.Add(new Character { Name = "Kratos", GameId = gow.Id, ImageUrl = "https://i.pinimg.com/1200x/46/0c/be/460cbebf9f459a1d82c3ff9ed54e05c4.jpg", Description = "Ex dios de la guerra de Esparta. Ahora vive en los reinos nórdicos con su hijo Atreus, intentando dejar atrás su pasado de sangre." });
        defaultCharacters.Add(new Character { Name = "Atreus", GameId = gow.Id, ImageUrl = "https://i.pinimg.com/1200x/1b/56/d9/1b56d9bf02cd5c426fba8eebbbc09d74.jpg", Description = "Hijo de Kratos y Faye. Aprende a ser un guerrero y descubre su verdadera naturaleza divina mientras acompaña a su padre." });
    }
    if (hollow != null)
        defaultCharacters.Add(new Character { Name = "El Caballero", GameId = hollow.Id, ImageUrl = "https://static.wikia.nocookie.net/ficcion-sin-limites/images/a/a8/TK.png/revision/latest?cb=20240428134831&path-prefix=es", Description = "Protagonista silencioso. Un pequeño vessel que explora las ruinas del reino de Hallownest en busca de su propósito." });
    if (celeste != null)
        defaultCharacters.Add(new Character { Name = "Madeline", GameId = celeste.Id, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/42/Celeste_character_Madeline.png", Description = "Una joven que decide escalar la montaña Celeste. En el camino afronta su ansiedad y sus miedos personificados en Part of Her." });

    foreach (var ch in defaultCharacters)
    {
        var existing = db.Characters.FirstOrDefault(c => c.Name == ch.Name);
        if (existing == null)
        {
            db.Characters.Add(ch);
        }
        else
        {
            existing.GameId = ch.GameId;
            existing.ImageUrl = ch.ImageUrl;
            existing.Description = ch.Description;
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
