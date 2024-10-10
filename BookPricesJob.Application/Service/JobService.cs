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

    public async Task<Job?> GetById(int id)
    {
        return await _unitOfWork.JobRepository.GetById(id);
    }

    public async Task<int> CreateJob(Job job)
    {
        var id = await _unitOfWork.JobRepository.Add(job);

        return id;
    }

    public async Task DeleteJob(int id)
    {
        _unitOfWork.JobRepository.Delete(id);
        await _unitOfWork.Complete();
    }
}
