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

    public async Task<Job?> GetById(string id)
    {
        return await _unitOfWork.JobRepository.GetById(id);
    }

    public async Task<string> CreateJob(Job job)
    {
        var id = await _unitOfWork.JobRepository.Add(job);
        await _unitOfWork.Complete();

        return id;
    }

    public async Task DeleteJob(string id)
    {
        await _unitOfWork.JobRepository.Delete(id);
        await _unitOfWork.Complete();
    }

    public async Task UpdateJob(Job job)
    {
        await _unitOfWork.JobRepository.Update(job);
        await _unitOfWork.Complete();
    }
}
