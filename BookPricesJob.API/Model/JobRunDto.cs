namespace BookPricesJob.API.Model;

public record JobRunDto(
    string Id,
    string JobId,
    string JobName,
    string Priority,
    string Status,
    DateTime Created,
    DateTime Updated,
    List<JobRunArgumentDto> Arguments,
    string? ErrorMessage);
