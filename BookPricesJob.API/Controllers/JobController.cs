using Microsoft.AspNetCore.Mvc;
using BookPricesJob.Application.Contract;
using BookPricesJob.API.Model;
using BookPricesJob.API.Mapper;
using Microsoft.AspNetCore.Authorization;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Authorize]
[Route("api/jobs")]
public sealed class JobController(IJobService jobService, ILogger<JobController> logger) : ControllerBase
{
    private readonly IJobService _jobService = jobService;
    private readonly ILogger<JobController> _logger = logger;

    [HttpGet]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<IList<JobListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Jobs()
    {
        var jobs =  await _jobService.GetJobs();
        var jobDtos = JobMapper.MapToList(jobs);

        return Ok(jobDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<JobDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Job([FromRoute] string id)
    {
        var job = await _jobService.GetJobById(id);
        if (job is null)
            return NotFound();

        var jobDto = JobMapper.MapToDto(job);

        return Ok(jobDto);
    }

    [HttpPost]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType<JobDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateJobRequest jobCreateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = JobMapper.MapToDomain(jobCreateRequest);
        var jobId = await _jobService.CreateJob(job);
        _logger.LogInformation("Job with id {JobRunId} created by {User}", jobId, User.Identity!.Name);

        job = await _jobService.GetJobById(jobId);
        if (job is null)
            return BadRequest();

        var jobDto = JobMapper.MapToDto(job);

        return CreatedAtAction(nameof(Job), new { id = jobId }, jobDto);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFull(
        [FromRoute] string id,
        [FromBody] UpdateJobFullRequest jobUpdateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != jobUpdateRequest.Id)
            return BadRequest();

        var job = await _jobService.GetJobById(id);
        if (job is null)
            return NotFound();

        var updatedJob = JobMapper.MapToDomain(jobUpdateRequest, job);
        await _jobService.UpdateJob(updatedJob);
        _logger.LogInformation("Job with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartial(
        [FromRoute] string id,
        [FromBody] UpdateJobPartialRequest jobUpdateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = await _jobService.GetJobById(id);
        if (job is null)
            return NotFound();

        if (jobUpdateRequest.IsActive.HasValue)
            job = job with { IsActive = jobUpdateRequest.IsActive.Value };
        if (jobUpdateRequest.Name != null)
            job = job with { Name = jobUpdateRequest.Name };
        if (jobUpdateRequest.Description != null)
            job = job with { Description = jobUpdateRequest.Description };

        await _jobService.UpdateJob(job);
        _logger.LogInformation("Job with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await _jobService.DeleteJob(id);
        _logger.LogInformation("Job with id {JobId} deleted by {User}", id, User.Identity!.Name);

        return Ok();
    }
}
