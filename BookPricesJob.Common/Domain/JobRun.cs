using System;

namespace BookPricesJob.Common.Domain;

public record JobRun(
    string Id,
    DateTime Created,
    DateTime Updated,
    JobRunStatus Status,
    IList<JobRunArgument> Arguments,
    string? ErrorMessage
);
