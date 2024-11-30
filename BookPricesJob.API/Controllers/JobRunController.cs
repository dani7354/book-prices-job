using BookPricesJob.API.Mapper;
using BookPricesJob.API.Model;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Authorize]
[Route("api/jobruns")]
public sealed class JobRunController(IJobService jobService, ILogger<JobRunController> logger) : ControllerBase
{
    private readonly IJobService _jobService = jobService;
    private readonly ILogger<JobRunController> _logger = logger;

    [HttpGet]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<IList<JobRunListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> JobRuns(
        [FromQuery] int? limit = null,
        [FromQuery] string? jobId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null)
    {
        JobRunStatus? jobRunStatus = Enum.TryParse<JobRunStatus>(
            status, true, out var statusEnum) ? statusEnum : null;
        JobRunPriority? jobRunPriority = Enum.TryParse<JobRunPriority>(
            priority, true, out var priorityEnum) ? priorityEnum : null;

        var jobRuns = await _jobService.FilterJobRuns(jobId, jobRunStatus, jobRunPriority, limit);
        var jobRunDtos = JobRunMapper.MapToListDto(jobRuns);

        return Ok(jobRunDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<JobRunDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobRun([FromRoute] string id)
    {
        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return NotFound(id);

        var job = await _jobService.GetJobById(jobRun.JobId);
        if (job is null)
            return NotFound(jobRun.JobId);
        var jobRunDto = JobRunMapper.MapToDto(jobRun, job.Name);

        return Ok(jobRunDto);
    }

    [HttpPost]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<JobRunDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJobRun([FromBody] CreateJobRunRequest createJobRunRequest)
    {
        var jobRun = JobRunMapper.MapToDomain(createJobRunRequest);
        var jobRunId = await _jobService.CreateJobRun(jobRun);
        _logger.LogInformation("Job Run created with id {JobRunId} by {User}", jobRunId, User.Identity!.Name);

        jobRun = await _jobService.GetJobRunById(jobRunId);
        if (jobRun is null)
            return BadRequest();

        var job = await _jobService.GetJobById(jobRun.JobId);
        if (job is null)
            return BadRequest();

        var jobRunResponseDto = JobRunMapper.MapToDto(jobRun, job.Name);

        return CreatedAtAction(nameof(JobRun), new { id = jobRunId }, jobRunResponseDto);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFull(
        [FromRoute] string id,
        [FromBody] UpdateJobRunFullRequest updateJobRunRequest)
    {
        if (id != updateJobRunRequest.JobRunId)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest();

        var updatedJobRun = JobRunMapper.MapToDomain(
            updateJobRunRequest,
            jobRun);

        await _jobService.UpdateJobRun(updatedJobRun);
        _logger.LogInformation("JobRun with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateJobRunPartial(
        [FromRoute] string id,
        [FromBody] UpdateJobRunPartialRequest updateJobRunRequest)
    {
        if (id == updateJobRunRequest.JobRunId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest();

        if (updateJobRunRequest.ErrorMessage is not null)
            jobRun = jobRun with { ErrorMessage = updateJobRunRequest.ErrorMessage };
        if (updateJobRunRequest.Status is not null)
            jobRun = jobRun with { Status = Enum.Parse<JobRunStatus>(updateJobRunRequest.Status) };
        if (updateJobRunRequest.Priority is not null)
            jobRun = jobRun with { Priority = Enum.Parse<JobRunPriority>(updateJobRunRequest.Priority) };
        if (updateJobRunRequest.Arguments.Any())
            jobRun = jobRun with { Arguments = updateJobRunRequest.Arguments.Select(
                x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values)).ToList() };

        await _jobService.UpdateJobRun(jobRun);
        _logger.LogInformation("JobRun with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteJobRun([FromRoute] string id)
    {
        await _jobService.DeleteJobRun(id);
        _logger.LogInformation("JobRun with id {JobRunId} deleted by {User}", id, User.Identity!.Name);

        return Ok();
    }
}
