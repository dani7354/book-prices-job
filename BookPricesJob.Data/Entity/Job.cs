using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.Data.Entity;

public class Job
{
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = null!;
    public bool IsActive { get; set; }
    [MaxLength(256)]
    public string Name { get; set; } = null!;
    [MaxLength(256)]
    public string Description { get; set; } = null!;
    public DateTime Created { get; set; }
    public List<JobRun> JobRuns { get; } = [];

    [ConcurrencyCheck]
    [MaxLength(36)]
    public string Version { get; set; } = null!;
}
