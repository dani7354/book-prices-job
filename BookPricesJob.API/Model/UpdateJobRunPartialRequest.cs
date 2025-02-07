using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobRunPartialRequest
{
    [Required]
    public string JobRunId { get; init; } = null!;
    public string? JobId { get; set; }
    public string? Priority { get; init; }
    public string? Status { get; init; }
    public List<JobRunArgumentDto> Arguments { get; init; } = [];
    public string? ErrorMessage { get; init; }
}
