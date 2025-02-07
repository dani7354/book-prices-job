using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;


public class RemoveRoleRequest
{
    [Required]
    public string UserName { get; } = null!;

    [Required]
    public string RoleName { get; } = null!;
}
