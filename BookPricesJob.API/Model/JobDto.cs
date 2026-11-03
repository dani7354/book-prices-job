namespace BookPricesJob.API.Model;

public record JobDto(
    string Id,
    bool IsActive,
    bool RunsWithArguments,
    string Name,
    string Description,
    string Version,
    DateTime Created,
    IList<JobRunListItemDto> JobRuns
);
