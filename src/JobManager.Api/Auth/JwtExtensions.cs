using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobManager.Api.Auth;

/// <summary>
/// Jwt token extensions methods
/// </summary>
[ExcludeFromCodeCoverage]
public static class JwtExtensions
{
    /// <summary>
    /// Configure Jtw token validation
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtBearerConfigurationOptions>();
    }
    
    /// <summary>
    /// Get customer details from Jwt Claims
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Guid GetUserId(this HttpContext context)
    {
        if (Guid.TryParse(
                context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                out var customerId))
        {
            return customerId;
        }

        return default;
    }
}