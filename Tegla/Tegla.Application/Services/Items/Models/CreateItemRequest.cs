using System.ComponentModel.DataAnnotations;

namespace Tegla.Application.Services.Items.Models;

public class CreateItemRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Range(1, 1200.00)]
    public double Price { get; set; }
    
    [Required]
    public string Make { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }
}
