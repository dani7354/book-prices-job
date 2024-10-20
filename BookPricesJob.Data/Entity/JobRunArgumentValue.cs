using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPricesJob.Data.Entity;

public class JobRunArgumentValue
{
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = null!;
    [MaxLength(36)]
    public string JobRunArgumentId { get; set; } = null!;
    [MaxLength(256)]
    public string Value { get; set; } = null!;
    [ForeignKey(nameof(JobRunArgumentId))]
    public JobRunArgument JobRunArgument { get; set; } = null!;
}
