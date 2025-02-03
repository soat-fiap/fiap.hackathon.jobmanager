using Amazon.CognitoIdentityProvider;

namespace JobManager.Cognito.Factory;

public interface ICognitoClientFactory
{
    IAmazonCognitoIdentityProvider CreateClient();
}
