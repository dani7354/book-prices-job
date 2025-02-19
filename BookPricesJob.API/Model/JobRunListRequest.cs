namespace BookPricesJob.API.Model;

public class JobRunListRequest
{
    public bool? Active { get; init; }
    public int? Limit { get; init; }
    public string? JobId { get; init; }
    public string[]? Status { get; init; }
    public string[]? Priority { get; init; }
}