using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using BookPricesJob.API.Validation;

namespace BookPricesJob.API.Model;

public class UpdateJobPartialRequest
{
    [Required]
    [MaxLength(36)]
    public string Id { get; init; } = null!;
    
    [Required] 
    [VersionFormat] 
    public string Version { get; init; } = null!;
    public bool? IsActive { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}
