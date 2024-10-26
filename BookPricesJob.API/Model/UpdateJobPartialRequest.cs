using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookPricesJob.API.Model;

public class UpdateJobPartialRequest
{
    [Required]
    [MaxLength(36)]
    public string Id { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
