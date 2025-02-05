using System.Diagnostics.CodeAnalysis;
using Hackathon.Video.SharedKernel;
using JobManager.Domain.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace JobManager.Application;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void AddUseCases(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUseCase<CreateJobDto, JobDto>, CreateJobUseCase>()
            .AddScoped<IUseCase<CreateJobVideoUploadUrl, string>, CreateVideoUploadUrlUseCase>()
            .AddScoped<IUseCase<Guid, IReadOnlyList<JobDto>>, GetUserJobsUseCase>()
            .AddScoped<IUseCase<UpdateJobStatusDto>, UpdateJobStatusUseCase>()
            .AddScoped<IUseCase<GetJobDetailDto, JobDto?>, GetJobDetailsUseCase>()
            .AddScoped<IUseCase<NotificationMessageDto>, NotifyCustomerUseCase>();
    }
}