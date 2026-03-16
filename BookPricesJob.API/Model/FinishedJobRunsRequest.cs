using BookPricesJob.API.Validation;
using Microsoft.AspNetCore.Mvc;

namespace BookPricesJob.API.Model;

public class FinishedJobRunsRequest
{
    [FromQuery(Name = "days"), DaysValue]
    public int Days { get; init; }
}