namespace WikiVideojuegos.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
