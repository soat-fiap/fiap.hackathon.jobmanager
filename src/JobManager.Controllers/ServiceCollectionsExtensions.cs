using System.Diagnostics.CodeAnalysis;
using JobManager.Controllers.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace JobManager.Controllers;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddJobManagerControllers(this IServiceCollection services)
    {
        services.AddScoped<IJobManagerService, JobManagerService>();
        services.AddScoped<IFileUpload, S3Service>();
    }
}
