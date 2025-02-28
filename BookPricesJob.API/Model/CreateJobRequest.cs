using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class CreateJobRequest
{
    [Required]
    public string Name { get; init; } = null!;
    
    [Required]
    public string Description { get; init; } = null!;
    
    [Required]
    public bool? IsActive { get; init; }
}
