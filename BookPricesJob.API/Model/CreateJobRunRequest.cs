using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;
public class CreateJobRunRequest
{
    [Required]
    public string JobId { get; init; } = null!;
    [Required]
    public string Priority { get; init; } = null!;
    public List<JobRunArgumentDto> Arguments { get; init; } = [];

}
