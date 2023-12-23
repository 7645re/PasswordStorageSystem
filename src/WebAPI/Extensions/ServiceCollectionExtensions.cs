using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPI.Filters;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddControllersWithErrorBehavior(this IServiceCollection services)
    {
        services
            .AddControllers(
                options =>
                    options.Filters.Add<ExceptionHandlerFilter>()
            );

        services
            .Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorsResponse = new ExceptionsResponse()
                    {
                        Exceptions = context.ModelState.Select(item =>
                            new ExceptionDetail()
                            {
                                Error = "Model validation error",
                                Parameter = item.Key,
                                Code = Guid.Empty,
                                Message =
                                    string.Join("\n", item.Value!.Errors.Select(e => e.ErrorMessage))
                            }
                        ).ToArray()
                    };
                    return new BadRequestObjectResult(errorsResponse);
                };
            });

        return services;
    }
    
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static AuthenticationBuilder AddJwt(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var key = Encoding.ASCII.GetBytes(configurationManager["Jwt:Key"]);
        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configurationManager["Jwt:Issuer"],
                ValidAudience = configurationManager["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });
    }
}