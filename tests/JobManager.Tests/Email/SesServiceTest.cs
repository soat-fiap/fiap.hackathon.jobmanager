using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using JetBrains.Annotations;
using JobManager.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JobManager.Tests.Email;

[TestSubject(typeof(SesService))]
public class SesServiceTest
{
    [Fact]
    public async Task SendEmailAsync_SendsEmailSuccessfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SesService>>();
        var sesClientMock = new Mock<IAmazonSimpleEmailService>();
        var emailOptionsMock = new Mock<IOptions<EmailOptions>>();
        emailOptionsMock.Setup(o => o.Value).Returns(new EmailOptions { SenderEmail = "sender@example.com" });
        var service = new SesService(loggerMock.Object, sesClientMock.Object, emailOptionsMock.Object);

        // Act
        await service.SendEmailAsync("Subject", "recipient@example.com", "Message");

        // Assert
        sesClientMock.Verify(client => client.SendEmailAsync(It.Is<SendEmailRequest>(req =>
            req.Destination.ToAddresses.Contains("recipient@example.com") &&
            req.Message.Subject.Data == "Subject" &&
            req.Message.Body.Html.Data == "Message" &&
            req.Source == "sender@example.com"
        ), default), Times.Once);
    }
}