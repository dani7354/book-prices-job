namespace BookPricesJob.Common.Exception;

public class JobRunNotFoundException(string jobRunId) : System.Exception($"JobRun with id {jobRunId} not found!")
{
}
