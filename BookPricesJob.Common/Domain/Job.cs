namespace BookPricesJob.Common.Domain;
using System;
using System.Collections.Generic;

public record Job(
    string? Id,
    bool IsActive,
    string Name,
    string Description,
    DateTime? Created,
    IList<JobRun> JobRuns);
