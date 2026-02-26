namespace WikiVideojuegos.Models;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? ImageUrl { get; set; }
    public int? Year { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
