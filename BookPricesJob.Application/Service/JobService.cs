using System;
using System.Collections.Generic;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class JobService(IUnitOfWork unitOfWork) : IJobService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IList<Job>> GetJobs()
    {
        return await _unitOfWork.JobRepository.GetJobs();
    }
}
