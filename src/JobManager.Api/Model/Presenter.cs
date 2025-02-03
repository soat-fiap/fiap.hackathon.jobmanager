using JobManager.Domain.Dto;

namespace JobManager.Api.Model;

public static class Presenter
{
    public static IReadOnlyCollection<ListJobResponse> ToListJobResponse(this IReadOnlyList<JobDto> job)
    {
        return job.Select(j => new ListJobResponse(j.JobId.ToString(), j.Status)).ToList();
    }
    
    public static ListJobResponse ToListJobResponse(this JobDto job)
    {
        return new ListJobResponse(job.JobId, job.Status);
    }
}