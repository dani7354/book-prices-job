
namespace BookPricesJob.Application.Service;

public static class CacheKeyGenerator
{
    private const string CacheVersion = "1";

    public static string GenerateJobKey(string id) => $"{CacheVersion}_Job_{id}";
    public static string GenerateJobRunKey(string id) => $"{CacheVersion}_JobRun_{id}";
    public static string GenerateJobListKey() => $"{CacheVersion}_JobIdList";
    public static string GenerateJobRunListKey() => $"{CacheVersion}_JobRunIdList";
}
