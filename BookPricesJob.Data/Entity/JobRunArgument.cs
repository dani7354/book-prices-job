using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data.Entity;

public class JobRunArgument
{
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = null!;
    public string JobRunId { get; set; } = null!;
    [MaxLength(32)]
    public string Type { get; set; } = null!;
    [MaxLength(256)]
    public string Name { get; set; } = null!;
    [ForeignKey(nameof(JobRunId))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public JobRun JobRun { get; init; } = null!;
    public List<JobRunArgumentValue> Values { get; set; } = [];

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
