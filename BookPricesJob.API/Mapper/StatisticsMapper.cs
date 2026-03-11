using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Mapper;

public static class StatisticsMapper
{
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    
    public static FinishedJobRunsStatisticsDto MapFinishedJobRunsToDto(
        IList<JobRunCountsByStatus> jobRunCountsByStatus,
        DateTime generatedAt)
    {
        var generatedAtFormatted = generatedAt.ToString(DateTimeFormat);
        
        var jobRunCountsByStatusDtos = jobRunCountsByStatus
            .Select(j => new JobRunCountDto(
                JobId: j.JobId, 
                JobName: j.JobName, 
                TotalJobRunCount: j.CountsByStatus.Sum(c => c.Value), 
                JobRunCountByStatus: j.CountsByStatus))
            .ToList();
        
        return new FinishedJobRunsStatisticsDto(generatedAtFormatted, jobRunCountsByStatusDtos);
    }
}