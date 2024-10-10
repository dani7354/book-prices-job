namespace BookPricesJob.API.Model;

public record JobListItemDto(
    bool IsActive,
    int Id,
    string Name,
    string Description,
    DateTime Created);
