using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class LoginRequest
{
    [Required]
    [MaxLength(256)]
    public string UserName { get; init; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string Password { get; init; } = null!;
}
