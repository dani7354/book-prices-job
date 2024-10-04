using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPricesJob.Data.Entity;

public class JobRunArgument
{
    [Key]
    public int Id { get; set; }
    public int JobRunId { get; set; }
    [MaxLength(32)]
    public string Type { get; set; } = null!;
    [MaxLength(256)]
    public string Name { get; set; } = null!;
    [MaxLength(256)]
    public string Value { get; set; } = null!;
    [ForeignKey(nameof(JobRunId))]
    public JobRun JobRun { get; init; } = null!;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
