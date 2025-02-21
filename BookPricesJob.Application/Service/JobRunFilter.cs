using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public record JobRunFilter(
    bool? Active,
    int? Limit,
    string? JobId,
    IEnumerable<JobRunPriority>? Priorities,
    IEnumerable<JobRunStatus>? Statuses,
    SortByOption SortBy,
    SortDirection SortDirection);