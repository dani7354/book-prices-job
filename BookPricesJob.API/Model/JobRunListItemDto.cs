namespace BookPricesJob.API.Model;

public record JobRunListItemDto(
    string Id,
    string JobId,
    string JobName,
    string Priority,
    string Status,
    DateTime Created,
    DateTime Updated);
