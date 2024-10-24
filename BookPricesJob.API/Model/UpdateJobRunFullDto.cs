using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobRunFullDto
{
    [Required]
    public string JobRunId { get; set; } = null!;
    [Required]
    public string JobId { get; set; } = null!;
    [Required]
    public string Priority { get; set; } = null!;
    [Required]
    public string Status { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public List<JobRunArgumentDto> Arguments { get; set; } = [];
}
