namespace BookPricesJob.Common.Domain;

public record JobRunArgument(
    string? Id,
    string Name,
    string Type,
    string[] Values);
