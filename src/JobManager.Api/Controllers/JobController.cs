using JobManager.Api.Auth;
using JobManager.Api.Model;
using JobManager.Controllers.Contracts;
using JobManager.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobController : ControllerBase
{
    private readonly IJobManagerService _jobManagerService;
    private readonly IFileUpload _fileUpload;
    private readonly ILogger<JobController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="jobManagerService">JobManagerService instance.</param>
    /// <param name="fileUpload">FileUpload instance.</param>
    /// <param name="logger">Logger instance.</param>
    public JobController(ILogger<JobController> logger, IJobManagerService jobManagerService, IFileUpload fileUpload)
    {
        _jobManagerService = jobManagerService;
        _fileUpload = fileUpload;
        _logger = logger;
    }

    /// <summary>
    /// Create a job
    /// </summary>
    /// <param name="createJobRequest">Request object containing job details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created job details</returns>
    [HttpPost]
    public async Task<ActionResult> Create(
        CreateJobRequest createJobRequest, CancellationToken cancellationToken)
    {
        var job = await _jobManagerService.CreateJobAsync(new CreateJobDto(HttpContext.GetUserId(), createJobRequest.Snapshots.Value));
        return Created("", job);
    }

    /// <summary>
    /// Get all jobs for a user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of jobs</returns>
    [HttpGet]
    public async Task<ActionResult<List<ListJobResponse>>> GetJobs(CancellationToken cancellationToken)
    {
        var jobs = await _jobManagerService.ListJobsAsync(HttpContext.GetUserId());

        return Ok(jobs.ToListJobResponse());
    }

    /// <summary>
    /// Get a specific job by ID
    /// </summary>
    /// <param name="id">Job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Job details</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ListJobResponse>> GetJob(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobManagerService.GetOneAsync(HttpContext.GetUserId(), id);
        return Ok(job.ToListJobResponse());
    }

    /// <summary>
    /// Get an upload link for a specific job
    /// </summary>
    /// <param name="id">Job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload link</returns>
    [HttpGet("{id:guid}/upload-link")]
    public async Task<ActionResult> GetUploadLink(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobManagerService.GetOneAsync(HttpContext.GetUserId(), id);
        if (job is null)
            return NotFound();

        var url = await _fileUpload.CreateUploadUrl(job);
        return Ok(new
        {
            uploadUrl = url
        });
    }

    /// <summary>
    /// Get an download link for a specific job
    /// </summary>
    /// <param name="id">Job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload link</returns>
    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult> GetDownloadLink(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobManagerService.GetOneAsync(HttpContext.GetUserId(), id);
        if (job is null)
            return NotFound();

        var url = await _fileUpload.CreateDownloadUrl(job);
        return Ok(new
        {
            uploadUrl = url
        });
    }
}