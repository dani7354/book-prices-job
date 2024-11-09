namespace BookPricesJob.Common.Exception;

public class JobNotFoundException(string id) : System.Exception($"Job with id {id} not found!")
{
}
