using System.Diagnostics.CodeAnalysis;
using JobManager.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace JobManager.DynamoDB;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigurePersistenceApp(this IServiceCollection services)
    {
        services.AddScoped<IJobRepository, JobRepository>();
    }
}