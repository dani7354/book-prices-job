namespace BookPricesJob.API.Model;


public class RemoveRoleRequest
{
    public string UserName { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}
