using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public JobRunArgument JobRunArgument { get; set; } = null!;
}
