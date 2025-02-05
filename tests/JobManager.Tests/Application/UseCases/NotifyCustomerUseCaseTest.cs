using JetBrains.Annotations;
using JobManager.Application;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using Moq;

namespace JobManager.Tests.Application.UseCases;

[TestSubject(typeof(NotifyCustomerUseCase))]
public class NotifyCustomerUseCaseTest
{
    [Fact]
    public async Task ExecuteAsync_SendsEmail_WhenUserExists()
    {
        var emailServiceMock = new Mock<IEmailService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var user = new UserDto(Guid.NewGuid(), "user@example.com");
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        var useCase = new NotifyCustomerUseCase(emailServiceMock.Object, userRepositoryMock.Object);
        var request = new NotificationMessageDto(user.Id, "Job Completed");

        await useCase.ExecuteAsync(request);

        emailServiceMock.Verify(service => service.SendEmailAsync("Job Completed", user.Email, request.Message),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotSendEmail_WhenUserDoesNotExist()
    {
        var emailServiceMock = new Mock<IEmailService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserDto?)null);
        var useCase = new NotifyCustomerUseCase(emailServiceMock.Object, userRepositoryMock.Object);
        var request = new NotificationMessageDto(Guid.NewGuid(), "Job Completed");

        await useCase.ExecuteAsync(request);

        emailServiceMock.Verify(
            service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenEmailServiceFails()
    {
        var emailServiceMock = new Mock<IEmailService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var user = new UserDto(Guid.NewGuid(), "user@example.com");
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        emailServiceMock
            .Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Email service failure"));
        var useCase = new NotifyCustomerUseCase(emailServiceMock.Object, userRepositoryMock.Object);
        var request = new NotificationMessageDto(user.Id, "Job Completed");

        await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(request));
    }
}