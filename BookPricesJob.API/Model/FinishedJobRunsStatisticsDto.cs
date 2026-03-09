namespace BookPricesJob.API.Model;

public record FinishedJobRunsStatisticsDto(
    string GeneratedAt, 
    IList<JobRunCountDto> JobRuns);