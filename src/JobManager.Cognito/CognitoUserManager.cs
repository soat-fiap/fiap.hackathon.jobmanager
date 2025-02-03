using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using JobManager.Cognito.Factory;
using JobManager.Domain.Contracts;
using JobManager.Domain.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobManager.Cognito;

public class CognitoUserManager(
    ICognitoClientFactory cognitoClientFactory,
    ILogger<CognitoUserManager> logger,
    IOptions<CognitoSettings> settings)
    : IUserRepository
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient = cognitoClientFactory.CreateClient();
    private readonly string _userPoolId = settings.Value.UserPoolId;

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            logger.LogInformation("Fetching user with Id {UserId}", userId);
            var response = await _cognitoClient.ListUsersAsync(new ListUsersRequest()
            {
                Filter = "sub=\"" + userId + "\"",
                UserPoolId = _userPoolId,
            });

            if (response.Users.Count > 0)
            {
                var attributes = response.Users[0].Attributes;

                var email = attributes.First(attr => attr.Name == "email").Value;
                var sub = attributes.First(attr => attr.Name == "sub").Value;
                return new UserDto(Guid.Parse(sub), email);
            }

            return default;
        }
        catch (UserNotFoundException)
        {
            logger.LogWarning("User not found.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user");
            throw;
        }
    }
}
