using Microsoft.AspNetCore.Mvc;
using BookPricesJob.Application.Contract;
using BookPricesJob.API.Model;
using BookPricesJob.API.Mapper;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class JobController : ControllerBase
{
    private readonly ILogger<JobController> _logger;
    private readonly IJobService _jobService;

    public JobController(
        ILogger<JobController> logger,
        IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs =  await _jobService.GetJobs();
        var jobDtos = JobMapper.MapToList(jobs);

        return Ok(jobDtos);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var job = await _jobService.GetJobById(id);
        if (job is null)
            return NotFound();

        var jobDto = JobMapper.MapToDto(job);

        return Ok(jobDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto jobCreateRequest)
    {
        var job = JobMapper.MapToDomain(jobCreateRequest);
        var jobId = await _jobService.CreateJob(job);

        job = await _jobService.GetJobById(jobId);

        return CreatedAtAction(nameof(Get), new { id = jobId }, job);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFull(
        [FromRoute] string id,
        [FromBody] UpdateJobFullDto jobUpdateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != jobUpdateRequest.Id)
            return BadRequest();

        var job = await _jobService.GetJobById(id);
        if (job is null)
            return BadRequest();

        var updatedJob = JobMapper.MapToDomain(jobUpdateRequest, job);
        await _jobService.UpdateJob(updatedJob);

        return Ok();

    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePartial(
        [FromRoute] string id,
        [FromBody] UpdateJobPartialDto jobUpdateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = await _jobService.GetJobById(id);
        if (job is null)
            return BadRequest();

        if (jobUpdateRequest.IsActive.HasValue)
            job = job with { IsActive = jobUpdateRequest.IsActive.Value };
        if (jobUpdateRequest.Name != null)
            job = job with { Name = jobUpdateRequest.Name };
        if (jobUpdateRequest.Description != null)
            job = job with { Description = jobUpdateRequest.Description };

        await _jobService.UpdateJob(job);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await _jobService.DeleteJob(id);

        return Ok();
    }
}
