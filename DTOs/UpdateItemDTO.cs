using System.ComponentModel.DataAnnotations;

namespace Catalog.DTOs
{
  public record UpdateItemDTO
  {
    [Required]
    public string Name { get; init; }
    [Required]
    [Range(0, 9999)]
    public decimal Price { get; init; }
  }
}