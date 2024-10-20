using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobDto
{
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
}
