namespace BookPricesJob.Common.Domain;

public record JobRunCountsByStatus(
    string JobId,
    string JobName,
    Dictionary<string, int> CountsByStatus,
    Dictionary<string, float> PercentagesByStatus)
{
    public static JobRunCountsByStatus CreateEmpty(string jobId, string jobName)
        => new(
            JobId: jobId,
            JobName: jobName,
            CountsByStatus: new Dictionary<string, int>(),
            PercentagesByStatus: new Dictionary<string, float>());
}