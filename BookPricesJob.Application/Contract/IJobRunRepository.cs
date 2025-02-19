namespace BookPricesJob.Application.Contract;

using BookPricesJob.Common.Domain;

public interface IJobRunRepository : IRepository<JobRun>
{
    public Task<IList<JobRun>> FilterBy(
        bool? active,
        int? limit,
        string? jobId,
        IEnumerable<JobRunStatus>? statuses,
        IEnumerable<JobRunPriority>? priorities);
}
