using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public JobRun JobRun { get; init; } = null!;
    public List<JobRunArgumentValue> Values { get; } = [];

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
