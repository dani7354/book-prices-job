namespace BookPricesJob.Application.Service;

public static class CacheKeyGenerator
{
    public static string GenerateJobKey(string id) => $"Job_{id}";
    public static string GenerateJobRunKey(string id) => $"JobRun_{id}";
}
