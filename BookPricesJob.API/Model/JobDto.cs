namespace BookPricesJob.API.Model
{
    public record JobDto(
        string Id,
        bool IsActive,
        string Name,
        string Description,
        DateTime Created,
        IList<JobRunListItemDto> JobRuns
    );
}
