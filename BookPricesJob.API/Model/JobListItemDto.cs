namespace BookPricesJob.API.Model;

public record JobListItemDto(
    bool IsActive,
    bool RunsWithArguments,
    string Id,
    string Name,
    string Description,
    DateTime Created);
