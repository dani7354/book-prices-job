namespace BookPricesJob.Common.Domain;

public record JobRunCountsByStatus(
    string JobId, 
    string JobName,
    Dictionary<string, int> CountsByStatus);