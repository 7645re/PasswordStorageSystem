using Domain.DTO;
using Domain.Mappers;
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

    public async Task<OperationResult<TokenInfo>> GetUserTokenAsync(UserSearch userSearch)
    {
        var userResult = await GetUserAsync(userSearch.UserLogin);
        if (!userResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = userResult.ErrorMessage
            };
        if (userResult.Result.Password != userSearch.Password)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = "Invalid password"
            };
        
        if (userResult.Result.Token == null)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = "User dont have token, please login"
            };
        if (userResult.Result.TokenExpire == null)
            throw new InvalidOperationException($"User {userResult.Result.Login} token doesnt have date of expire");
        
        var tokenIsValid = await _tokenBlackListRepository.ValidateTokenAsync(userResult.Result.Token);
        if (!tokenIsValid)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = "Your Access Token Is Blacklisted"
            };

        return new OperationResult<TokenInfo>
        {
            IsSuccess = true,
            Result = new TokenInfo(userResult.Result.Token, (DateTimeOffset)userResult.Result.TokenExpire)
        };
    }

    public async Task<OperationResult<IEnumerable<User>>> GetAllUsersAsync()
    {
        var userEntities = await _userRepository.GetAllUsersAsync();
        return new OperationResult<IEnumerable<User>>
        {
            IsSuccess = true,
            Result = userEntities.Select(ue => ue.ToUser())
        };
    }

    public async Task<OperationResult<User>> GetUserAsync(string userLogin)
    {
        var userEntity = await _userRepository.GetUserAsync(userLogin);
        if (userEntity == null)
            return new OperationResult<User>
            {
                IsSuccess = false,
                ErrorMessage = $"User {userLogin} doesnt exist"
            };

        return new OperationResult<User>
        {
            IsSuccess = true,
            Result = userEntity.ToUser()
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

    public async Task<OperationResult<TokenInfo>> CreateUserAsync(UserCreate userCreate)
    {
        var userResult = await GetUserAsync(userCreate.Login);
        if (userResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = $"User with login {userCreate.Login} already exist"
            };

        var validateResult = _userValidator.Validate(userCreate);
        if (!validateResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        var tokenInfo = _tokenService.GenerateToken(userCreate.Login);
        await _userRepository.CreateUserAsync(userCreate.ToUserEntity(tokenInfo.Token, tokenInfo.Expire));

        return new OperationResult<TokenInfo>
        {
            IsSuccess = true,
            Result = tokenInfo
        };
    }

    public async Task<OperationResult> ChangePasswordAsync(string userLogin, string newPassword)
    {
        var userResult = await GetUserAsync(userLogin);
        if (!userResult.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = userResult.ErrorMessage
            };

        if (userResult.Result.Password == newPassword)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "You already have have this password"
            };
        
        var validateResult = _userValidator.ValidatePassword(newPassword);
        if (!validateResult.IsSuccess)
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = validateResult.ErrorMessage
            };

        await _userRepository.ChangePasswordAsync(userLogin, newPassword);
        return new OperationResult
        {
            IsSuccess = true
        };
    }
}