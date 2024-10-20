using BookPricesJob.API.Model;
using BookPricesJob.Application.Contract;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Controllers;

[Route("api/jobruns")]
public class JobRunController(IJobService jobService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;

    [HttpGet]
    public IActionResult JobRuns([FromQuery] int limit = 100, [FromQuery] string? status = null)
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult JobRun([FromRoute] int id)
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult CreateJobRun([FromBody] CreateJobRunDto jobRunDto)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateJobRun([FromRoute] string id, [FromBody] UpdateJobRunDto jobRunDto)
    {
        return Ok();
    }
}
