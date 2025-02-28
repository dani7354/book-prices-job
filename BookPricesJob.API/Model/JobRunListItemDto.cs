namespace BookPricesJob.API.Model;

public record JobRunListItemDto(
    string Id,
    string JobId,
    string JobName,
    string Priority,
    string Status,
    string Version,
    DateTime Created,
    DateTime Updated,
    IList<JobRunArgumentDto> Arguments);
