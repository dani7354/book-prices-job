using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Test.Setup;

public static class HttpClientHelper
{
    private const string MediaType = "application/json";
    public static async Task<HttpResponseMessage> PostJob(HttpClient client, CreateJobRequest job)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(job),
            Encoding.UTF8,
            MediaType);

        var response = await client.PostAsync(Constant.JobsBaseEndpoint, content);
        response.EnsureSuccessStatusCode();

        return response;
    }

    public static async Task<HttpResponseMessage> PatchJob(HttpClient client, UpdateJobPartialRequest update)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(update),
            Encoding.UTF8,
            MediaType);

        var response = await client.PatchAsync($"{Constant.JobsBaseEndpoint}/{update.Id}", content);
        response.EnsureSuccessStatusCode();

        return response;
    }
    
    public static async Task<JobRunDto> CreateJobRunForJob(
        HttpClient client,
        string jobId,
        JobRunPriority priority = JobRunPriority.Normal)
    {
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = priority.ToString()
        };

        var content = CreateStringPayload(jobRunPayload);

        var responseCreateJobRun = await client.PostAsync(Constant.JobRunsBaseEndpoint, content);
        var jobRunDto = await responseCreateJobRun.Content.ReadFromJsonAsync<JobRunDto>();
        Assert.NotNull(jobRunDto);

        return jobRunDto;
    }

    public static async Task<HttpResponseMessage> PatchJobRun(HttpClient client, UpdateJobRunPartialRequest update)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(update),
            Encoding.UTF8,
            MediaType);

        var response = await client.PatchAsync($"{Constant.JobRunsBaseEndpoint}/{update.JobRunId}", content);
        response.EnsureSuccessStatusCode();

        return response;
    }
    
    public static StringContent CreateStringPayload<T>(T payload)
    {
        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            MediaType);
    }
}
