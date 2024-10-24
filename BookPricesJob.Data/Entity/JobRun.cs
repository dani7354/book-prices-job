using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data.Entity;

public class JobRun
{
    [Key]
    [MaxLength(36)]
    public string Id { get; set; } = null!;

    [MaxLength(36)]
    public string JobId { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    [MaxLength(32)]
    public string Status { get; set; } = null!;
    [MaxLength(32)]
    public string Priority { get; set; } = null!;
    [MaxLength(512)]
    public string? ErrorMessage { get; set; }
    [ForeignKey(nameof(JobId))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Job Job { get; set; } = null!;
    public List<JobRunArgument> Arguments { get; } = [];

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
