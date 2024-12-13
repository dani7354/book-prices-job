using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class CreateJobRequest
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public bool? IsActive { get; set; }
}
