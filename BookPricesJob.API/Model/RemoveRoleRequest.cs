using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;


public class RemoveRoleRequest
{
    [Required]
    public string UserName { get; init; } = null!;

    [Required]
    public string RoleName { get; init; } = null!;
}
