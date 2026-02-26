namespace WikiVideojuegos.DTOs;

public record GameDto(int Id, string Name, string? ImageUrl, int? Year, string? Description, DateTime CreatedAt);

public record CreateGameRequest(string Name, string? ImageUrl, int? Year, string? Description);

public record UpdateGameRequest(string Name, string? ImageUrl, int? Year, string? Description);
