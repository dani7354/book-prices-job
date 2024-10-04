using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.Data.Entity;

public class Job
{
    [Key]
    public int Id { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(256)]
    public string Name { get; set; } = null!;
    [MaxLength(256)]
    public string Description { get; set; } = null!;
    public DateTime Created { get; set; }
    public List<JobRun> JobRuns { get; } = [];

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
