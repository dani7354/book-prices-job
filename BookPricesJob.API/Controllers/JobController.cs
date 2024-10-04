using Microsoft.AspNetCore.Mvc;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Route("job")]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> _logger;
    private readonly IJobService _jobService;

    public JobController(ILogger<JobController> logger, IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Jobs()
    {
        var jobs =  await _jobService.GetJobs();

        return Ok(jobs);
    }
}
