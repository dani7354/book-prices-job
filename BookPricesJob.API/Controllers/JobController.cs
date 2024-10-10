using Microsoft.AspNetCore.Mvc;
using BookPricesJob.Application.Contract;
using BookPricesJob.API.Model;
using BookPricesJob.API.Mapper;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobController : ControllerBase
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
    public async Task<IActionResult> GetAllJobs()
    {
        var jobs =  await _jobService.GetJobs();
        var jobDtos = JobMapper.MapToList(jobs);

        return Ok(jobDtos);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetJob([FromRoute]int id)
    {
        var job = await _jobService.GetById(id);
        if (job is null)
            return NotFound();

        var jobDto = JobMapper.MapToDto(job);

        return Ok(jobDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto jobCreateRequest)
    {
        var job = JobMapper.MapToDomain(jobCreateRequest);
        var jobId = await _jobService.CreateJob(job);

        job = await _jobService.GetById(jobId);

        return CreatedAtAction(nameof(GetJob), new { id = jobId }, job);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateJob([FromRoute] int id, [FromBody] UpdateJobDto jobUpdateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != jobUpdateRequest.Id)
            return BadRequest();

        var job = JobMapper.MapToDomain(jobUpdateRequest);

        await _jobService.UpdateJob(job);

    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteJob([FromRoute]int id)
    {
        await _jobService.DeleteJob(id);

        return Ok();
    }
}
