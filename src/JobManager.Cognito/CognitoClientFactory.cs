using Amazon.CognitoIdentityProvider;
using JobManager.Cognito.Factory;

namespace JobManager.Cognito;

public class CognitoClientFactory : ICognitoClientFactory
{
    public IAmazonCognitoIdentityProvider CreateClient()
    {
        return new AmazonCognitoIdentityProviderClient();
    }
}
