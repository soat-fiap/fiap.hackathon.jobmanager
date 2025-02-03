namespace JobManager.Domain.Contracts;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string destination, string  message);   
}