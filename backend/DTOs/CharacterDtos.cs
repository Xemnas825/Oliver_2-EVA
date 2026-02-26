namespace WikiVideojuegos.DTOs;

public record CharacterDto(int Id, string Name, int GameId, string? ImageUrl, string? Description, DateTime CreatedAt);

public record CharacterDetailDto(int Id, string Name, int GameId, string? GameName, string? ImageUrl, string? Description, DateTime CreatedAt);

public record CreateCharacterRequest(string Name, int GameId, string? ImageUrl, string? Description);

public record UpdateCharacterRequest(string Name, int GameId, string? ImageUrl, string? Description);
