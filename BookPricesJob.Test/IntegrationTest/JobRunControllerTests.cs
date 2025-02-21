using System.Net;
using System.Net.Http.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Test.IntegrationTest;

public class JobRunControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public JobRunControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _client = factory.CreateClient();
    }

    public static IEnumerable<object[]> JobRunStatusesAndPriorities =>
    [
        [JobRunPriority.High, JobRunStatus.Running],
        [JobRunPriority.Low, JobRunStatus.Failed],
        [JobRunPriority.Normal, JobRunStatus.Completed],
    ];

    public static IEnumerable<object[]> JobRunArguments =>
    [
        [
            new List<JobRunArgumentDto>()
        ],
        [
            new List<JobRunArgumentDto>
            {
                new() {
                    Name = "Arg1",
                    Type = "String",
                    Values = ["Value1"]
                }
            }
        ],
        [
            new List<JobRunArgumentDto>
            {
                new() {
                    Name = "Arg1",
                    Type = "Integer",
                    Values = ["1", "500"]
                },
                new() {
                    Name = "Arg2",
                    Type = "String",
                    Values = ["Value1"]
                }
            }
        ],
    ];

    private async Task<JobRunDto> CreateJobRunForJob(
        string jobId,
        JobRunPriority priority = JobRunPriority.Normal)
    {
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = priority.ToString()
        };

        var content = HttpClientHelper.CreateStringPayload(jobRunPayload);

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);
        var jobRunDto = await responseCreateJobRun.Content.ReadFromJsonAsync<JobRunDto>();
        Assert.NotNull(jobRunDto);

        return jobRunDto;
    }

    private async Task<JobRunDto> CreateJobWithJobRun(
        JobRunPriority priority = JobRunPriority.Normal,
        bool isActive = true)
    {
        var jobRequest = new CreateJobRequest
        {
            Name = TestData.CreateJobRequestOne.Name,
            Description = TestData.CreateJobRequestOne.Description,
            IsActive = isActive
        };
        
        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobRequest);
        var jobDto = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobId = jobDto!.Id;
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = priority.ToString()
        };

        var content = HttpClientHelper.CreateStringPayload(jobRunPayload);

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);
        var jobRunDto = await responseCreateJobRun.Content.ReadFromJsonAsync<JobRunDto>();
        Assert.NotNull(jobRunDto);

        return jobRunDto;
    }

    [Fact]
    public async Task JobRuns_NoJobsOrJobRuns_ReturnsSuccessEmptyArray()
    {
        var response = await _client.GetAsync(Constant.JobRunsBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Empty(jobRuns);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
    
    [Fact]
    public async Task JobRuns_Filtering_ReturnsSuccessWithListOfJobRuns()
    {
        var jobRunDto = await CreateJobWithJobRun();
        await CreateJobRunForJob(jobRunDto.JobId);
        await CreateJobRunForJob(jobRunDto.JobId);
        await CreateJobRunForJob(jobRunDto.JobId);
        
        var response = await _client.GetAsync(Constant.JobRunsBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Equal(4, jobRuns.Length);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
    
    [Fact]
    public async Task JobRuns_Filtering_ReturnsSuccessWithListOfJobRunsFilteredByPriorityAndJobId()
    {
        var jobRunDto = await CreateJobWithJobRun();
        await CreateJobRunForJob(jobRunDto.JobId);
        await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.Low);
        await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.High);

        var url = $"{Constant.JobRunsBaseEndpoint}";
        url += $"?jobId={jobRunDto.JobId}";
        url += $"&priority={JobRunPriority.Normal}";
        url += $"&priority={JobRunPriority.High}";
        
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Equal(3, jobRuns.Length);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
    
    [Fact]
    public async Task JobRuns_Filtering_ReturnsSuccessWithListOfJobRunsFilteredByJobActive()
    {
        await CreateJobWithJobRun(isActive: true);
        await CreateJobWithJobRun(isActive: true);
        var jobId = (await CreateJobWithJobRun(isActive: true)).JobId;
        await HttpClientHelper.PatchJob(
            _client, 
            new UpdateJobPartialRequest { Id = jobId, IsActive = false });

        var url = $"{Constant.JobRunsBaseEndpoint}?active=true";
        
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Equal(2, jobRuns.Length);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
    
    [Fact]
    public async Task JobRuns_Ordering_ReturnsSuccessListOdJobRunsOrderedByPriorityDesc()
    {
        var jobRunDto = await CreateJobWithJobRun();
        await CreateJobRunForJob(jobRunDto.JobId);
        await CreateJobRunForJob(jobRunDto.JobId);
        await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.Low);
        await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.High);
        await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.High);

        var url = $"{Constant.JobRunsBaseEndpoint}?sortBy=priority&sortDirection=descending";
        
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Equal(6, jobRuns.Length);
        Assert.Equal(JobRunPriority.High.ToString(), jobRuns.First().Priority);
        Assert.Equal(JobRunPriority.Normal.ToString(), jobRuns[2].Priority);
        Assert.Equal(JobRunPriority.Low.ToString(), jobRuns.Last().Priority);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }
    
    [Fact]
    public async Task JobRuns_Ordering_ReturnsSuccessListOdJobRunsOrderedByStatusAsc()
    {
        var jobRunDto = await CreateJobWithJobRun();
        var jobRunId = (await CreateJobRunForJob(jobRunDto.JobId)).Id;
        await HttpClientHelper.PatchJobRun(_client, new UpdateJobRunPartialRequest
        {
            JobId = jobRunDto.JobId,
            JobRunId = jobRunId,
            Status = JobRunStatus.Running.ToString()
        });
        
        jobRunId = (await CreateJobRunForJob(jobRunDto.JobId)).Id;
        await HttpClientHelper.PatchJobRun(_client, new UpdateJobRunPartialRequest
        {
            JobId = jobRunDto.JobId,
            JobRunId = jobRunId,
            Status = JobRunStatus.Failed.ToString()
        });
        
        jobRunId = (await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.Low)).Id;
        await HttpClientHelper.PatchJobRun(_client, new UpdateJobRunPartialRequest
        {
            JobId = jobRunDto.JobId,
            JobRunId = jobRunId,
            Status = JobRunStatus.Failed.ToString()
        });
        
        jobRunId = (await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.High)).Id;
        await HttpClientHelper.PatchJobRun(_client, new UpdateJobRunPartialRequest
        {
            JobId = jobRunDto.JobId,
            JobRunId = jobRunId,
            Status = JobRunStatus.Pending.ToString()
        });
        
        jobRunId = (await CreateJobRunForJob(jobRunDto.JobId, JobRunPriority.High)).Id;
        await HttpClientHelper.PatchJobRun(_client, new UpdateJobRunPartialRequest
        {
            JobId = jobRunDto.JobId,
            JobRunId = jobRunId,
            Status = JobRunStatus.Completed.ToString()
        });

        var url = $"{Constant.JobRunsBaseEndpoint}?sortBy=status&sortDirection=ascending";
        
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Equal(6, jobRuns.Length);
        Assert.Equal(JobRunStatus.Completed.ToString(), jobRuns.First().Status);
        Assert.Equal(JobRunStatus.Failed.ToString(), jobRuns[1].Status);
        Assert.Equal(JobRunStatus.Pending.ToString(), jobRuns[3].Status);
        Assert.Equal(JobRunStatus.Running.ToString(), jobRuns.Last().Status);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }

    [Fact]
    public async Task Create_NewJobRun_ReturnsSuccessAndCreatedObject()
    {
        var responseCreateJob = await HttpClientHelper.PostJob(_client, TestData.CreateJobRequestOne);
        var jobDto = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobId = jobDto!.Id;
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = JobRunPriority.Normal.ToString()
        };

        var content = HttpClientHelper.CreateStringPayload(jobRunPayload);

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.Created, responseCreateJobRun.StatusCode);
        Assert.Equal(
            "application/json; charset=utf-8",
            responseCreateJobRun.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Create_NoJobsExist_ReturnsBadRequest()
    {
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = Guid.NewGuid().ToString(),
            Priority = "Normal"
        };

        var content = HttpClientHelper.CreateStringPayload(jobRunPayload);

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.BadRequest, responseCreateJobRun.StatusCode);
    }

    [Theory]
    [MemberData(nameof(JobRunStatusesAndPriorities))]
    public async Task UpdateFull_JobRunStatusAndPriority_ReturnsUpdatedStatusAndPriority(
        JobRunPriority newPriority,
        JobRunStatus newStatus)
    {
        var jobRunDto = await CreateJobWithJobRun();

        var updateJobRunPayload = new UpdateJobRunFullRequest()
        {
            JobRunId = jobRunDto.Id,
            JobId = jobRunDto.JobId,
            Priority = newPriority.ToString(),
            Status = newStatus.ToString()
        };

        var updateContent = HttpClientHelper.CreateStringPayload(updateJobRunPayload);

        var responseUpdateJobRun = await _client.PutAsync(
            $"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}",
            updateContent);

        var responseGetJobRunUpdated = await _client.GetAsync($"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}");
        var jobRunUpdated = await responseGetJobRunUpdated.Content.ReadFromJsonAsync<JobRunDto>();

        Assert.Equal(HttpStatusCode.OK, responseUpdateJobRun.StatusCode);
        Assert.NotNull(jobRunUpdated);
        Assert.Equal(newStatus.ToString(), jobRunUpdated.Status);
        Assert.Equal(newPriority.ToString(), jobRunUpdated.Priority);
    }

    [Theory]
    [MemberData(nameof(JobRunArguments))]
    public async Task UpdateFull_JobRunStatusArguments_ReturnsSuccessAndCorrectArguments(
        List<JobRunArgumentDto> arguments)
    {
        var jobRunDto = await CreateJobWithJobRun();

        var updateJobRunPayload = new UpdateJobRunFullRequest()
        {
            JobRunId = jobRunDto.Id,
            JobId = jobRunDto.JobId,
            Priority = jobRunDto.Priority,
            Status = jobRunDto.Status,
            Arguments = arguments
        };

        var updateContent = HttpClientHelper.CreateStringPayload(updateJobRunPayload);

        var responseUpdateJobRun = await _client.PutAsync(
            $"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}",
            updateContent);

        var responseGetJobRunUpdated = await _client.GetAsync($"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}");
        var jobRunUpdated = await responseGetJobRunUpdated.Content.ReadFromJsonAsync<JobRunDto>();

        Assert.Equal(HttpStatusCode.OK, responseUpdateJobRun.StatusCode);
        Assert.NotNull(jobRunUpdated);
        Assert.Equal(arguments.Count, jobRunUpdated.Arguments.Count);

        var firstArgumentOriginal = arguments.FirstOrDefault();
        if (firstArgumentOriginal is not null)
        {
            var firstArgumentUpdated = jobRunUpdated.Arguments.FirstOrDefault();
            Assert.NotNull(firstArgumentUpdated);
            Assert.Equal(firstArgumentOriginal.Name, firstArgumentUpdated.Name);
            Assert.Equal(firstArgumentOriginal.Type, firstArgumentUpdated.Type);
            Assert.Equal(firstArgumentOriginal.Values, firstArgumentUpdated.Values);
        }
    }

    [Fact]
    public async Task UpdateFull_InvalidJobId_ReturnsBadRequest()
    {
        var jobRunDto = await CreateJobWithJobRun();

        var updateJobRunPayload = new UpdateJobRunFullRequest()
        {
            JobRunId = jobRunDto.Id,
            JobId = Guid.NewGuid().ToString(),
            Priority = jobRunDto.Priority,
            Status = jobRunDto.Status
        };

        var updateContent = HttpClientHelper.CreateStringPayload(updateJobRunPayload);

        var responseUpdateJobRun = await _client.PutAsync(
            $"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}",
            updateContent);

        Assert.Equal(HttpStatusCode.BadRequest, responseUpdateJobRun.StatusCode);
    }

    [Theory]
    [MemberData(nameof(JobRunStatusesAndPriorities))]
    public async Task UpdatePartial_JobRunPriorityAndStatus_ReturnsUpdatedStatusAndPriority(
        JobRunPriority newPriority,
        JobRunStatus newStatus)
    {
        var jobRunDto = await CreateJobWithJobRun();

        var updateJobRunPayload = new UpdateJobRunPartialRequest()
        {
            JobRunId = jobRunDto.Id,
            Status = newStatus.ToString(),
            Priority = newPriority.ToString()
        };

        var updateContent = HttpClientHelper.CreateStringPayload(updateJobRunPayload);

        var responseUpdateJobRun = await _client.PatchAsync(
            $"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}",
            updateContent);

        var responseGetJobRunUpdated = await _client.GetAsync($"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}");
        var jobRunUpdated = await responseGetJobRunUpdated.Content.ReadFromJsonAsync<JobRunDto>();

        Assert.Equal(HttpStatusCode.OK, responseUpdateJobRun.StatusCode);
        Assert.NotNull(jobRunUpdated);
        Assert.Equal(newStatus.ToString(), jobRunUpdated.Status);
        Assert.Equal(newPriority.ToString(), jobRunUpdated.Priority);
    }

    [Fact]
    public async Task Delete_JobRun_ReturnsSuccess()
    {
        var jobRunDto = await CreateJobWithJobRun();

        var responseDeleteJobRun = await _client.DeleteAsync($"{Constant.JobRunsBaseEndpoint}/{jobRunDto.Id}");

        Assert.Equal(HttpStatusCode.OK, responseDeleteJobRun.StatusCode);
    }

    [Fact]
    public async Task Delete_InvalidJobRunId_ReturnsBadRequest()
    {
        await CreateJobWithJobRun();

        var responseDeleteJobRun = await _client.DeleteAsync($"{Constant.JobRunsBaseEndpoint}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, responseDeleteJobRun.StatusCode);
    }
}
