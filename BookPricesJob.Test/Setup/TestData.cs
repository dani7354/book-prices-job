using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Test.Setup;

public static class TestData
{
    public static CreateJobRequest CreateJobRequestOne => new()
    {
        Name = "Test Job 1",
        Description = "Test Job 1 described here",
        IsActive = true
    };

   
}
