using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobRunPartialRequest
{
    [Required]
    public string JobRunId { get; set; } = null!;
    public string? JobId { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public List<JobRunArgumentDto> Arguments { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
