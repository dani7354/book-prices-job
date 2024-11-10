namespace BookPricesJob.Common.Exception;

public class UpdateFailedException(string id, string? message = null)
    : System.Exception($"Update failed for object with id {id}! {message}")
{
}
