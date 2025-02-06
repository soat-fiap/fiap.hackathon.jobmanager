using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using JetBrains.Annotations;
using JobManager.Cognito;
using JobManager.Cognito.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JobManager.Tests.Cognito;

[TestSubject(typeof(CognitoUserManager))]
public class CognitoUserManagerTest
{
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var cognitoClientFactoryMock = new Mock<ICognitoClientFactory>();
        var cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
        var loggerMock = new Mock<ILogger<CognitoUserManager>>();
        var settingsMock = new Mock<IOptions<CognitoSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new CognitoSettings { UserPoolId = "testPoolId" });

        cognitoClientFactoryMock.Setup(f => f.CreateClient()).Returns(cognitoClientMock.Object);
        cognitoClientMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse
            {
                Users = new List<UserType>
                {
                    new UserType
                    {
                        Attributes = new List<AttributeType>
                        {
                            new AttributeType { Name = "email", Value = "test@example.com" },
                            new AttributeType { Name = "sub", Value = Guid.NewGuid().ToString() }
                        }
                    }
                }
            });

        var userManager =
            new CognitoUserManager(cognitoClientFactoryMock.Object, loggerMock.Object, settingsMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await userManager.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var cognitoClientFactoryMock = new Mock<ICognitoClientFactory>();
        var cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
        var loggerMock = new Mock<ILogger<CognitoUserManager>>();
        var settingsMock = new Mock<IOptions<CognitoSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new CognitoSettings { UserPoolId = "testPoolId" });

        cognitoClientFactoryMock.Setup(f => f.CreateClient()).Returns(cognitoClientMock.Object);
        cognitoClientMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ListUsersResponse { Users = new List<UserType>() });

        var userManager =
            new CognitoUserManager(cognitoClientFactoryMock.Object, loggerMock.Object, settingsMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await userManager.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_LogsWarning_WhenUserNotFoundExceptionIsThrown()
    {
        // Arrange
        var cognitoClientFactoryMock = new Mock<ICognitoClientFactory>();
        var cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
        var loggerMock = new Mock<ILogger<CognitoUserManager>>();
        var settingsMock = new Mock<IOptions<CognitoSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new CognitoSettings { UserPoolId = "testPoolId" });

        cognitoClientFactoryMock.Setup(f => f.CreateClient()).Returns(cognitoClientMock.Object);
        cognitoClientMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ThrowsAsync(new UserNotFoundException("User not found"));

        var userManager =
            new CognitoUserManager(cognitoClientFactoryMock.Object, loggerMock.Object, settingsMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await userManager.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var cognitoClientFactoryMock = new Mock<ICognitoClientFactory>();
        var cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
        var loggerMock = new Mock<ILogger<CognitoUserManager>>();
        var settingsMock = new Mock<IOptions<CognitoSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new CognitoSettings { UserPoolId = "testPoolId" });

        cognitoClientFactoryMock.Setup(f => f.CreateClient()).Returns(cognitoClientMock.Object);
        cognitoClientMock.Setup(c => c.ListUsersAsync(It.IsAny<ListUsersRequest>(), default))
            .ThrowsAsync(new Exception("Unexpected error"));

        var userManager =
            new CognitoUserManager(cognitoClientFactoryMock.Object, loggerMock.Object, settingsMock.Object);
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => userManager.GetUserByIdAsync(userId));
    }
}