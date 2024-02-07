using System.ComponentModel.DataAnnotations;

namespace Tegla.Application.Services.Items.Models;

public class UpdateItemRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public string Make { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }
}
