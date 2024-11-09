namespace BookPricesJob.Common.Exception;

public class JobNotFoundException(string jobId) : System.Exception($"Job with id {jobId} not found!")
{
}
