using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Common;

public static class AuthExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer(
                serviceName: "keycloak",
                realm: "overflow",
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "overflow";
                    
                    // 1. The external URL your browser/clients use to authenticate
                    options.Authority = "http://id.overflow.local/realms/overflow";
                    
                    // 2. The internal Docker Compose endpoint used by the container to download signing keys.
                    // This bypasses any routing/DNS issues inside the Docker network.
                    //options.MetadataAddress = "http://keycloak:8080/realms/overflow/.well-known/openid-configuration";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = [
                            "http://localhost:6001/realms/overflow",
                            "http://keycloak:8080/realms/overflow",
                            "http://keycloak/realms/overflow",
                            "http://id.overflow.local/realms/overflow"
                        ],
                        ValidateIssuer = true
                    };
                });

        services.AddAuthorizationBuilder();

        return services;
    }
}