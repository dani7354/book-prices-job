namespace BookPricesJob.Application.Contract;

using System.Reflection.Metadata;
using BookPricesJob.Common.Domain;

public interface IJobRunRepository : IRepository<JobRun>
{
    public Task<IList<JobRun>> FilterBy(
        string? jobId,
        JobRunStatus? status,
        JobRunPriority? priority,
        int? limit);

}
