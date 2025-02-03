using System.Diagnostics.CodeAnalysis;
using JobManager.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace JobManager.Email;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void AddEmailService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEmailService, SesService>();
    }
}