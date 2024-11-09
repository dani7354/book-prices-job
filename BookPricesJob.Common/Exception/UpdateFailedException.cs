namespace BookPricesJob.Common.Exception;

public class UpdateFailedException(string message) : System.Exception(message)
{
}
