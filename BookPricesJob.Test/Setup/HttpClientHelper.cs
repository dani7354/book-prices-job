using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;

namespace BookPricesJob.Test.Setup;

public static class HttpClientHelper
{
    public static async Task<HttpResponseMessage> PostJob(HttpClient client, CreateJobRequest job)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(job),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(Constant.JobsBaseEndpoint, content);
        response.EnsureSuccessStatusCode();

        return response;
    }
}
