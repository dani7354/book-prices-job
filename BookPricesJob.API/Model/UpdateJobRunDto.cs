using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;

public class UpdateJobRunDto
{
    [Required]
    public string JobRunId { get; set; } = null!;
    [Required]
    public string Priority { get; set; } = null!;
    [Required]
    public int Status { get; set; }
    public List<JobRunArgumentDto> Arguments { get; set; } = [];
}
