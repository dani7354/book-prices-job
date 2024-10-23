using BookPricesJob.API.Model;
using BookPricesJob.Application.Contract;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Controllers;

[Route("api/jobruns")]
public sealed class JobRunController(IJobService jobService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;

    [HttpGet]
    public async Task<IActionResult> JobRuns([FromQuery] int limit = 100, [FromQuery] string? status = null)
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> JobRun([FromRoute] int id)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobRun([FromBody] CreateJobRunDto jobRunDto)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobRun([FromRoute] string id, [FromBody] UpdateJobRunDto jobRunDto)
    {
        return Ok();
    }
}
