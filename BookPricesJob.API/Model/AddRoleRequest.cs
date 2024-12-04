using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class AddRoleRequest
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;
}
