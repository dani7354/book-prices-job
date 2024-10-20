namespace BookPricesJob.API.Model;

public record JobListItemDto(
    bool IsActive,
    string Id,
    string Name,
    string Description,
    DateTime Created);
