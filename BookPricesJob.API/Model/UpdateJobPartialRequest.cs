using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookPricesJob.API.Model;

public class UpdateJobPartialRequest
{
    [Required]
    [MaxLength(36)]
    public string Id { get; set; } = null!;
    public bool? IsActive { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}
