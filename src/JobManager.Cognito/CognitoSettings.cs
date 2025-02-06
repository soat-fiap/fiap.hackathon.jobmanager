using System.Diagnostics.CodeAnalysis;

namespace JobManager.Cognito;

/// <summary>
/// Cognito User Pool settings
/// </summary>
[ExcludeFromCodeCoverage]
public class CognitoSettings
{
    /// <summary>
    /// User Pool Id
    /// </summary>
    public string UserPoolId { get; set; } = string.Empty;
}
