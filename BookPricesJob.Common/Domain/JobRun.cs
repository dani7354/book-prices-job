using System;

namespace BookPricesJob.Common.Domain;

public record JobRun(
    string Id,
    string JobId,
    DateTime Created,
    DateTime Updated,
    JobRunStatus Status,
    IList<JobRunArgument> Arguments,
    string? ErrorMessage
);
