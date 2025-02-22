namespace BookPricesJob.API.Model;

public record JobRunDto(
    string Id,
    string JobId,
    string JobName,
    string Priority,
    string Status,
    string Version,
    DateTime Created,
    DateTime Updated,
    List<JobRunArgumentDto> Arguments,
    string? ErrorMessage)
{
   
}
