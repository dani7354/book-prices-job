using BookPricesJob.API.Mapper;
using BookPricesJob.API.Model;
using BookPricesJob.Application.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Controllers;

[Authorize]
[ApiController]
[Route("api/statistics")]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet("finished-job-runs")]
    [Authorize(Policy = Constant.JobRunnerPolicy)]
    [ProducesResponseType<FinishedJobRunsStatisticsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> FinishedJobRuns()
    {
        var finishedJobRuns = await statisticsService.GetJobRunCountsByJob();
        var responseGeneratedTime = DateTime.Now;
        
        var finishedJobRunsCountsDto = StatisticsMapper.MapFinishedJobRunsToDto(
            finishedJobRuns, 
            responseGeneratedTime);
        
        return Ok(finishedJobRunsCountsDto);
    }
}