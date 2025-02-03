namespace JobManager.Api.Model;

public class CreateJobRequest
{
    public int? Snapshots { get; init; } = 20;
}