namespace BookPricesJob.API.Model;

public class JobRunArgumentDto
{
    public string Name { get; init; } = null!;
    public string Type { get; init; } = null!;
    public string[] Values { get; init; } = null!;
}
