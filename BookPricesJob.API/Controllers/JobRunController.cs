using BookPricesJob.API.Mapper;
using BookPricesJob.API.Model;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Controllers;

[Route("api/jobruns")]
public sealed class JobRunController(IJobService jobService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;

    [HttpGet]
    public async Task<IActionResult> JobRuns([FromQuery] int limit = 100, [FromQuery] string? status = null)
    {
        var jobRuns = await _jobService.GetJobRuns();
        var jobRunDtos = JobRunMapper.MapToListDto(jobRuns);

        return Ok(jobRunDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> JobRun([FromRoute] string id)
    {
        var jobRun = await _jobService.GetJobRunById(id);
        if (jobRun is null)
            return NotFound($"JobRun with id {id} not found!");

        var jobRunDto = JobRunMapper.MapToDto(jobRun);

        return Ok(jobRunDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobRun([FromBody] CreateJobRunDto jobRunDto)
    {
        var jobRun = JobRunMapper.MapToDomain(jobRunDto);
        var jobRunId = await _jobService.CreateJobRun(jobRun);

        jobRun = await _jobService.GetJobRunById(jobRunId);
        if (jobRun is null)
            return BadRequest();

        return CreatedAtAction(nameof(JobRun), new { id = jobRunId }, jobRun);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobRunFull([FromRoute] string id, [FromBody] UpdateJobRunFullDto jobRunDto)
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
    public async Task<IActionResult> UpdateJobRunPartial([FromRoute] string id, [FromBody] UpdateJobRunPartialDto jobRunDto)
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
            jobRun = jobRun with { Arguments = jobRunDto.Arguments.Select(x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values)).ToList() };

        await _jobService.UpdateJobRun(jobRun);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobRun([FromRoute] string id)
    {
        await _jobService.DeleteJobRun(id);
        return Ok();
    }
}
