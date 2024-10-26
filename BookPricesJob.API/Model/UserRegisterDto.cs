using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UserRegisterDto
{
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = null!;
    [Required]
    [MaxLength(128)]
    public string Password { get; set; } = null!;
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;
}
