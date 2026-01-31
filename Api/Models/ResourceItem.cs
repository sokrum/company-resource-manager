namespace Api.Models;

public class ResourceItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // np. Laptop, Room, ProjectDoc
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
