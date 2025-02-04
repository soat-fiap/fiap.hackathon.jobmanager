using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace JobManager.Api.Auth;

[ExcludeFromDescription]
public class JwtBearerConfigurationOptions(IConfiguration configuration)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigureSectionName ="JwtOptions";
    
    public string Name => JwtBearerDefaults.AuthenticationScheme;

    public void Configure(JwtBearerOptions options)
    {
        configuration.GetSection(ConfigureSectionName).Bind(options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
       Configure(options);
    }
}