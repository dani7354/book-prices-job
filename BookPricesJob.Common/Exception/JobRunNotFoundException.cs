namespace BookPricesJob.Common.Exception;

public class JobRunNotFoundException(string id) : System.Exception($"JobRun with id {id} not found!")
{
}
