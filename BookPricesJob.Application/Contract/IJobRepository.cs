using System.Collections.Generic;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobRepository : IRepository<Job>
{
    public Task<IList<Job>> GetJobs();
}
