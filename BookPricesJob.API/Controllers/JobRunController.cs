using BookPricesJob.API.Extension;
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
    [HttpGet]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<IList<JobRunListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> JobRuns([FromQuery] JobRunListRequest jobRunListRequest)
    {
        var jobRunFilter = JobRunMapper.MapFilterRequestToFilter(jobRunListRequest);
        var jobRuns = await jobService.FilterJobRuns(jobRunFilter);
        
        var jobRunDtos = JobRunMapper.MapToListDto(jobRuns);

        return Ok(jobRunDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<JobRunDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobRun([FromRoute] string id)
    {
        var jobRun = await jobService.GetJobRunById(id);
        if (jobRun is null)
            return NotFound(id);

        var job = await jobService.GetJobById(jobRun.JobId);
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobId = createJobRunRequest.JobId;
        var job = await jobService.GetJobById(jobId);
        if (job is null || !job.IsActive)
            return BadRequest($"Job ({jobId}) not found or perhaps it's not active!");
        
        var jobRun = JobRunMapper.MapToDomain(createJobRunRequest);
        var jobRunId = await jobService.CreateJobRun(jobRun);
        logger.LogInformation("Job Run created with id {JobRunId} by {User}", jobRunId, User.Identity!.Name);

        jobRun = await jobService.GetJobRunById(jobRunId);
        if (jobRun is null)
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

        var job = await jobService.GetJobById(updateJobRunRequest.JobId);
        if (job is null)
            return BadRequest();

        var jobRun = await jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest();

        var updatedJobRun = JobRunMapper.MapToDomain(
            updateJobRunRequest,
            jobRun);

        await jobService.UpdateJobRun(updatedJobRun);
        logger.LogInformation("JobRun with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartial(
        [FromRoute] string id,
        [FromBody] UpdateJobRunPartialRequest updateJobRunRequest)
    {
        if (id != updateJobRunRequest.JobRunId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jobRun = await jobService.GetJobRunById(id);
        if (jobRun is null)
            return BadRequest();
        
        var newPriopriority = updateJobRunRequest.Priority;
        var newStatus = updateJobRunRequest.Status;
        var arguments = updateJobRunRequest.Arguments;

        if (!string.IsNullOrEmpty(updateJobRunRequest.ErrorMessage))
            jobRun = jobRun with { ErrorMessage = updateJobRunRequest.ErrorMessage };
        if (newStatus is not null && Enum.TryParse<JobRunStatus>(newStatus, out var status))
            jobRun = jobRun with { Status = status };
        if (newPriopriority is not null && Enum.TryParse<JobRunPriority>(newPriopriority, out var priority))
            jobRun = jobRun with { Priority = priority };
        if (updateJobRunRequest.Arguments.Any())
            jobRun = jobRun with 
            { 
                Arguments = arguments.Select(
                x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values))
                .ToList() 
            };

        await jobService.UpdateJobRun(jobRun);
        logger.LogInformation("JobRun with id {JobRunId} updated by {User}", id, User.Identity!.Name);

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Constant.JobManagerPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var jobRun = await jobService.GetJobRunById(id);
        if (jobRun is null)
            return NotFound();

        await jobService.DeleteJobRun(id);
        logger.LogInformation("JobRun with id {JobRunId} deleted by {User}", id, User.Identity!.Name);

        return Ok();
    }
}
