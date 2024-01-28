using System.Text;
using Domain.Factories;
using Domain.Repositories.CredentialCountBySecurityLevelRepository;
using Domain.Repositories.CredentialHistoryRepository;
using Domain.Repositories.CredentialRepository;
using Domain.Repositories.CredentialsByResourceRepository;
using Domain.Repositories.UserRepository;
using Domain.Services.CredentialService;
using Domain.Services.TokenService;
using Domain.Services.UserService;
using Domain.Validators.CredentialValidator;
using Domain.Validators.UserValidator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPI.Filters;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddControllersWithErrorBehavior(
        this IServiceCollection services)
    {
        services
            .AddControllers(
                options =>
                    options.Filters.Add<ExceptionHandlerFilter>()
            );

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorsResponse = new ExceptionsResponse
                {
                    Exceptions = context.ModelState.Select(item =>
                        new ExceptionDetail
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

    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICredentialService, CredentialService>();
        services.AddSingleton<ITokenService, TokenService>();
        return services;
    }

    public static IServiceCollection AddFactories(
        this IServiceCollection services)
    {
        services.AddSingleton<ICassandraSessionFactory, CassandraSessionFactory>();
        return services;
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddSingleton<ICredentialByResourceRepository, CredentialByResourceRepository>();
        services.AddSingleton<ICredentialHistoryRepository, CredentialHistoryRepository>();
        services.AddSingleton<ICredentialCountBySecurityLevelRepository, CredentialCountBySecurityLevelRepository>();
        services.AddSingleton<ICredentialRepository, CredentialRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        return services;
    }

    public static IServiceCollection AddValidators(
        this IServiceCollection services)
    {
        services.AddSingleton<ICredentialValidator, CredentialValidator>();
        services.AddSingleton<IUserValidator, UserValidator>();
        return services;
    }

    public static IServiceCollection AddCORS(
        this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("www-authenticate");
            });
        });
    }

    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
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

    public static AuthenticationBuilder AddJwt(
        this IServiceCollection services,
        ConfigurationManager configurationManager)
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