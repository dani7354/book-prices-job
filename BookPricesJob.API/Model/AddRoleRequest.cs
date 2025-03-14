using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class AddRoleRequest
{
    [Required]
    public string UserName { get; init; } = null!;

    [Required]
    public string RoleName { get; init; } = null!;
}
