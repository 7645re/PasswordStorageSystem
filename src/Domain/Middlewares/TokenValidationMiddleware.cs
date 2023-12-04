using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Domain.Middlewares;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _tokenBlackList;
    private readonly ILogger<TokenValidationMiddleware> _logger;
    private readonly ITokenBlackListRepository _tokenBlackListRepository;

    public TokenValidationMiddleware(
        RequestDelegate next,
        IMemoryCache tokenBlackList,
        ILogger<TokenValidationMiddleware> logger,
        ITokenBlackListRepository tokenBlackListRepository)
    {
        _next = next;
        _tokenBlackList = tokenBlackList;
        _logger = logger;
        _tokenBlackListRepository = tokenBlackListRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _logger.LogInformation($"Attempting to log without token");
            return;
        }

        if (_tokenBlackList.TryGetValue(token, out _)
            || !await _tokenBlackListRepository.ValidateTokenAsync(token))
        {
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _logger.LogInformation($"Attempting to log in with a {token} that is blacklisted");
            return;
        }

        await _next(context);
    }
}