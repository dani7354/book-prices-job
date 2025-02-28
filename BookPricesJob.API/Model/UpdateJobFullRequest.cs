using System.ComponentModel.DataAnnotations;
using BookPricesJob.API.Validation;

namespace BookPricesJob.API.Model;

public class UpdateJobFullRequest
{
    [Required]
    public bool? IsActive { get; init; }
    
    [Required]
    public string Id { get; init; } = null!;
    
    [Required]
    public string Name { get; init; } = null!;
    
    [Required]
    public string Description { get; init; } = null!;
    
    [Required]
    [VersionFormat]
    public string Version { get; init; } = null!;
}
