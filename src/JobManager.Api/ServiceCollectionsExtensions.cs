using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SimpleEmail;
using JobManager.Api.BackgroundService;
using JobManager.Application;
using JobManager.Cognito;
using JobManager.Controllers;
using JobManager.DynamoDB;
using JobManager.Email;
using JobManager.Masstransit;

namespace JobManager.Api;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void IoCSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddUseCases();
        serviceCollection.ConfigurePersistenceApp();
        serviceCollection.AddJobManagerControllers();
        serviceCollection.AddDynamoDbConnection(configuration);
        serviceCollection.ConfigureDispatcher();
        serviceCollection.AddS3Connection();
        serviceCollection.AddEmailService();
        serviceCollection.AddSimpleEmailServiceClient(configuration);
        serviceCollection.ConfigureCognito();
        serviceCollection.ConfigureVideoReceivedQueue();
    }
    
    public static void ConfigureHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDynamoDbHealthCheck("Job");
    }

    private static void AddDynamoDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient());
    }
    
    private static void AddS3Connection(this IServiceCollection services)
    {
        services.AddScoped<IAmazonS3>(_ => new AmazonS3Client());
    }
    
    private static void AddSimpleEmailServiceClient(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddScoped<IAmazonSimpleEmailService>(_ => new AmazonSimpleEmailServiceClient());
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("EmailOptions"))
            .ValidateDataAnnotations();
    }
}  