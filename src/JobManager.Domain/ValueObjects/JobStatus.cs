namespace JobManager.Domain.ValueObjects;

public enum JobStatus
{
    Open = 0,
    InProgress,
    Completed,
    Error,
    Canceled
}