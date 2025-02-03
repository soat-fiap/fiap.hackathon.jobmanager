using Hackathon.Video.SharedKernel;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;

namespace JobManager.Application;

public class NotifyCustomerUseCase(IEmailService emailService, IUserRepository userRepository) : IUseCase<NotificationMessageDto>
{
    public async Task ExecuteAsync(NotificationMessageDto request)
    {
        var user = await userRepository.GetUserByIdAsync(Guid.Parse(request.UserId));
        if (user is null)
        {
            return;
        }

        await emailService.SendEmailAsync("Job Completed", user.Email, request.Message);
    }
}