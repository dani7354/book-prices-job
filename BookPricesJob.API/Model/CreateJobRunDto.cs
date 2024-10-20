using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Model;
public class CreateJobRunDto
{
    [Required]
    public string JobId { get; set; } = null!;
    [Required]
    public string Priority { get; set; } = null!;
    public List<JobRunArgumentDto> Arguments { get; set; } = [];

}
