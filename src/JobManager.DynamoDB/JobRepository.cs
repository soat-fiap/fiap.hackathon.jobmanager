using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Entities;

namespace JobManager.DynamoDB;

public class JobRepository : IJobRepository
{
    private const string JobsTable = "Jobs";
    private readonly IAmazonDynamoDB _database;
    private readonly DynamoDBContext _context;

    public JobRepository(IAmazonDynamoDB database)
    {
        _database = database;
        _context = new DynamoDBContext(_database);
    }

    public async Task SaveAsync(Job job)
    {
        await _context.SaveAsync(job);
    }

    public async Task UpdateAsync(Job job)
    {
        await _context.SaveAsync(job);
    }

    public async Task<IReadOnlyList<Job>> GetJobsAsync(string userId)
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("UserId", ScanOperator.Equal, userId)
        };
        var search = _context.ScanAsync<Job>(conditions);
        var jobs = await search.GetRemainingAsync();
        return jobs;
    }

    public async Task<Job> GetJobAsync(string userId, Guid jobId)
    {
        var job = await _context.LoadAsync<Job>(userId, jobId.ToString());
        return job;
    }
}