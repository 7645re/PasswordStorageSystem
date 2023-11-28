using Domain.DTO;
using Domain.Models;
using Domain.Repositories;
using Domain.Validators;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;
    private readonly ITokenService _tokenService;
    private readonly ITokenBlackListRepository _tokenBlackListRepository;

    public UserService(IUserRepository userRepository, IUserValidator userValidator, ITokenService tokenService,
        ITokenBlackListRepository tokenBlackListRepository)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _tokenService = tokenService;
        _tokenBlackListRepository = tokenBlackListRepository;
    }

    public async Task<OperationResult<TokenInfo>> GetUserByLoginAndPasswordAsync(string userLogin, string password)
    {
        var userResult = await GetUserAsync(userLogin);
        if (!userResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = userResult.ErrorMessage
            };
        if (userResult.Result.Password != password)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = "Invalid password"
            };

        var validToken = await _tokenBlackListRepository.ValidateTokenAsync(userResult.Result.Token);
        if (!validToken) return new OperationResult<TokenInfo>
        {
            IsSuccess = false,
            ErrorMessage = "Your Access Token Is Blacklisted"
        };
        
        return new OperationResult<TokenInfo>
        {
            IsSuccess = true,
            Result = new TokenInfo(userResult.Result.Token, userResult.Result.TokenExpire)
        };
    }

    public async Task<OperationResult<IEnumerable<UserEntity>>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return new OperationResult<IEnumerable<UserEntity>>
        {
            IsSuccess = true,
            Result = users
        };
    }

    public async Task<OperationResult<UserEntity>> GetUserAsync(string userLogin)
    {
        var user = await _userRepository.GetUserAsync(userLogin);
        if (user == null)
            return new OperationResult<UserEntity>
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist",
            };

        return new OperationResult<UserEntity>
        {
            IsSuccess = true,
            Result = user
        };
    }

    public async Task<OperationResult> DeleteUserAsync(string userLogin)
    {
        var userResult = await GetUserAsync(userLogin);
        if (!userResult.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = userResult.ErrorMessage
            };

        await _userRepository.DeleteUserAsync(userLogin);
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    public async Task<OperationResult<TokenInfo>> CreateUserAsync(string userLogin, string password)
    {
        var userResult = await GetUserAsync(userLogin);
        if (userResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = $"User with login {userLogin} already exist"
            };

        var validateResult = _userValidator.Validate(userLogin, password);
        if (!validateResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var token = _tokenService.GenerateToken(userLogin);
        await _userRepository.CreateUserAsync(new UserEntity
        {
            Login = userLogin,
            Password = password,
            Token = token.Token,
            TokenExpire = token.Expire
        });
        return new OperationResult<TokenInfo>
        {
            IsSuccess = true,
            Result = token
        };
    }

    public async Task<OperationResult> ChangePasswordAsync(string userLogin, string password)
    {
        var userResult = await GetUserAsync(userLogin);
        if (!userResult.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = userResult.ErrorMessage
            };

        var validateResult = _userValidator.Validate(userLogin, password);
        if (!validateResult.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        if (userResult.Result.Password == password)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "You already have have this password"
            };

        await _userRepository.ChangePasswordAsync(userLogin, password);
        return new OperationResult
        {
            IsSuccess = true
        };
    }
}