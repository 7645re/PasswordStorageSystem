using Domain.DTO;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Validators;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, IUserValidator userValidator, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _tokenService = tokenService;
    }

    public async Task<OperationResult<TokenInfo>> GetUserTokenAsync(UserLogIn userLogIn)
    {
        var userResult = await GetUserAsync(userLogIn.Login);
        if (!userResult.IsSuccess)
            return new OperationResult<TokenInfo> { IsSuccess = false, ErrorMessage = userResult.ErrorMessage };

        if (userResult.Result?.Password != userLogIn.Password)
            return new OperationResult<TokenInfo> { IsSuccess = false, ErrorMessage = "Invalid password" };

        if (userResult.Result?.TokenExpire.ToLocalTime() < DateTimeOffset.Now)
        {
            var newToken = _tokenService.GenerateAccessToken(userLogIn.Login);
            return new OperationResult<TokenInfo> { IsSuccess = true, Result = newToken };
        }
        
        return new OperationResult<TokenInfo>
        {
            IsSuccess = true, Result = new TokenInfo(userResult.Result.Token, userResult.Result.TokenExpire)
        };
    }

    public async Task<OperationResult<User>> GetUserAsync(string userLogin)
    {
        var userEntity = await _userRepository.GetUserAsync(userLogin);
        if (userEntity == null)
            return new OperationResult<User> { IsSuccess = false, ErrorMessage = $"User {userLogin} doesnt exist" };

        return new OperationResult<User> { IsSuccess = true, Result = userEntity.ToUser() };
    }

    public async Task<OperationResult> DeleteUserAsync(string userLogin)
    {
        var userResult = await GetUserAsync(userLogin);
        if (!userResult.IsSuccess)
            return new OperationResult { IsSuccess = false, ErrorMessage = userResult.ErrorMessage };

        await _userRepository.DeleteUserAsync(userLogin);
        return new OperationResult { IsSuccess = true };
    }

    public async Task<OperationResult<TokenInfo>> CreateUserAsync(UserCreate userCreate)
    {
        var userResult = await GetUserAsync(userCreate.Login);
        if (userResult.IsSuccess)
            return new OperationResult<TokenInfo>
            {
                IsSuccess = false, ErrorMessage = $"User with login {userCreate.Login} already exist"
            };

        var validateResult = _userValidator.Validate(userCreate);
        if (!validateResult.IsSuccess)
            return new OperationResult<TokenInfo> { IsSuccess = false, ErrorMessage = validateResult.ErrorMessage };

        var accessTokenInfo = _tokenService.GenerateAccessToken(userCreate.Login);
        await _userRepository.CreateUserAsync(userCreate.ToUserEntity(accessTokenInfo));

        return new OperationResult<TokenInfo> { IsSuccess = true, Result = accessTokenInfo };
    }

    public async Task<OperationResult> ChangePasswordAsync(UserChangePassword userChangePassword)
    {
        var userResult = await GetUserAsync(userChangePassword.Login);
        if (!userResult.IsSuccess)
            return new OperationResult { IsSuccess = false, ErrorMessage = userResult.ErrorMessage };

        if (userResult.Result?.Password == userChangePassword.NewPassword)
            return new OperationResult { IsSuccess = false, ErrorMessage = "You already have have this password" };

        var validateResult = _userValidator.ValidatePassword(userChangePassword.NewPassword);
        if (!validateResult.IsSuccess)
            return new OperationResult { IsSuccess = false, ErrorMessage = validateResult.ErrorMessage };

        await _userRepository.ChangePasswordAsync(userChangePassword.Login, userChangePassword.NewPassword);
        return new OperationResult { IsSuccess = true };
    }
}