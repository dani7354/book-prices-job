using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobRunFullRequest
{
    [Required]
    public string JobRunId { get; init; } = null!;
    [Required]
    public string JobId { get; init; } = null!;
    [Required]
    public string Priority { get; init; } = null!;
    [Required]
    public string Status { get; init; } = null!;
    public string? ErrorMessage { get; init; }
    public List<JobRunArgumentDto> Arguments { get; init; } = [];
}
