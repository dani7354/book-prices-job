namespace BookPricesJob.API.Model;

public record JobRunCountDto(
    string JobId,
    string JobName,
    int TotalJobRunCount,
    IDictionary<string, int> JobRunCountByStatus,
    IDictionary<string, float> JobRunPercentageByStatus);
