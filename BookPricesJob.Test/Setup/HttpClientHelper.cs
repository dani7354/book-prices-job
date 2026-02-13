using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;

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
