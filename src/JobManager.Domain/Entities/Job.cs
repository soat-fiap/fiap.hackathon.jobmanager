using JobManager.Domain.ValueObjects;

namespace JobManager.Domain.Entities;

public class Job
{
    public string Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int Snapshots { get; init; }

    public string VideoPath { get; set; } = string.Empty;

    public JobStatus Status { get; private set; } = JobStatus.Open;

    public int SnapshotsProcessed { get; set; } = 0;

    public void SetStatus(JobStatus status)
    {
        Status = status;
    }

    public void IncreaseSnapshotsProcessed()
    {
        if (SnapshotsProcessed >= Snapshots || Status != JobStatus.InProgress) return;
        
        SnapshotsProcessed++;
        Status = SnapshotsProcessed == Snapshots ? JobStatus.Completed : Status;
    }
}