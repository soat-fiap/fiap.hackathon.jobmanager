using System.Diagnostics.CodeAnalysis;
using JobManager.Cognito.Factory;
using JobManager.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JobManager.Cognito;

[ExcludeFromCodeCoverage]
public class CognitoSettingsSetup(IConfiguration configuration) : IConfigureOptions<CognitoSettings>
{
    public void Configure(CognitoSettings options)
    {
        configuration
            .GetSection(nameof(CognitoSettings))
            .Bind(options);
    }
}

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigureCognito(this IServiceCollection services)
    {
        services.ConfigureOptions<CognitoSettingsSetup>();
        services.AddSingleton<ICognitoClientFactory, CognitoClientFactory>();
        services.AddScoped<IUserRepository, CognitoUserManager>();
    }
}
