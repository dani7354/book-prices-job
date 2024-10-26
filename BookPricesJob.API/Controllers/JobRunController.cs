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
    [ProducesResponseType<JobRunDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobRun([FromRoute] string id)
    {
        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return NotFound($"JobRun with id {id} not found!");

        var job = await _jobService.GetJobById(jobRun.JobId);
        if (job is null)
            return NotFound($"Could not find related Job with id {jobRun.JobId} for JobRun!");

        var jobRunDto = JobRunMapper.MapToDto(jobRun, job.Name);

        return Ok(jobRunDto);
    }

    [HttpPost]
    [ProducesResponseType<JobRunDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJobRun([FromBody] CreateJobRunDto jobRunDto)
    {
        var jobRun = JobRunMapper.MapToDomain(jobRunDto);
        var jobRunId = await _jobService.CreateJobRun(jobRun);

        jobRun = await _jobService.GetJobRunById(jobRunId);
        if (jobRun is null)
            return BadRequest($"JobRun was not created!");

        var job = await _jobService.GetJobById(jobRun.JobId);
        if (job is null)
            return BadRequest($"Related Job with {jobRun.JobId} not found!");

        var jobRunResponseDto = JobRunMapper.MapToDto(jobRun, job.Name);

        return CreatedAtAction(nameof(JobRun), new { id = jobRunId }, jobRunResponseDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateJobRunFull(
        [FromRoute] string id,
        [FromBody] UpdateJobRunFullDto jobRunDto)
    {
        if (id != jobRunDto.JobRunId)
            return BadRequest("JobRunIds do not match!");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest($"JobRun with id {id} not found!");

        var updatedJobRun = JobRunMapper.MapToDomain(jobRunDto, jobRun);
        await _jobService.UpdateJobRun(updatedJobRun);

        return Ok();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateJobRunPartial(
        [FromRoute] string id,
        [FromBody] UpdateJobRunPartialDto jobRunDto)
    {
        if (id == jobRunDto.JobRunId)
            return BadRequest("JobRunIds do not match!");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest($"JobRun with id {id} not found!");

        if (jobRunDto.ErrorMessage is not null)
            jobRun = jobRun with { ErrorMessage = jobRunDto.ErrorMessage };
        if (jobRunDto.Status is not null)
            jobRun = jobRun with { Status = Enum.Parse<JobRunStatus>(jobRunDto.Status) };
        if (jobRunDto.Priority is not null)
            jobRun = jobRun with { Priority = Enum.Parse<JobRunPriority>(jobRunDto.Priority) };
        if (jobRunDto.Arguments.Any())
            jobRun = jobRun with { Arguments = jobRunDto.Arguments.Select(
                x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values)).ToList() };

        await _jobService.UpdateJobRun(jobRun);

        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteJobRun([FromRoute] string id)
    {
        await _jobService.DeleteJobRun(id);

        return Ok();
    }
}
