using System;

namespace BookPricesJob.Common.Domain;

public enum JobRunStatus
{
    Completed = 0,
    Failed = 1,
    Pending = 2,
    Running = 3,
}
