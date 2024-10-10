using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class LoginRequestModel
{
    [Required]
    [MaxLength(255)]
    public string? Email { get; set; }
    [Required]
    [MaxLength(128)]
    public string? Password { get; set; }

}
