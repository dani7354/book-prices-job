namespace BookPricesJob.API.Model;

public class JobRunArgumentDto
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}