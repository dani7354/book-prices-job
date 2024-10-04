namespace BookPricesJob.Common.Domain;

public record JobRunArgument(
    int Id,
    string Name,
    string Type,
    string Value);
