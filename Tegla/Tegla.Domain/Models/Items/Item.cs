namespace Tegla.Domain.Models.Items;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string Make { get; set; }
    public string Origin { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
