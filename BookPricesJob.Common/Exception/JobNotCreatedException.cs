namespace BookPricesJob.Common.Exception;

public class JobNotCreatedException(string? message) : System.Exception(message)
{
}
