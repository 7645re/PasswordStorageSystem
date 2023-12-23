using Domain.DTO;
using Domain.DTO.User;
using Domain.Mappers;
using Domain.Repositories.UserRepository;
using Domain.Services.TokenService;
using Domain.Validators.UserValidator;

namespace Domain.Services.UserService;

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

    public async Task<TokenInfo> GetUserTokenAsync(UserLogIn userLogIn)
    {
        var user = await _userRepository.GetUserAsync(userLogIn.Login);

        if (user.Password != userLogIn.Password)
            throw new Exception("Invalid password");

        if (user.AccessTokenExpire.ToLocalTime() < DateTimeOffset.Now)
            return _tokenService.GenerateAccessToken(userLogIn.Login);

        return new TokenInfo(user.AccessToken, user.AccessTokenExpire);
    }

    public async Task<User> GetUserAsync(string userLogin)
    {
        var userEntity = await _userRepository.GetUserAsync(userLogin);
        return userEntity.ToUser();
    }

    public async Task DeleteUserAsync(string userLogin)
    {
        await _userRepository.DeleteUserAsync(userLogin);
    }

    public async Task<TokenInfo> CreateUserAsync(UserCreate userCreate)
    {
        _userValidator.Validate(userCreate);

        var accessTokenInfo = _tokenService.GenerateAccessToken(userCreate.Login);
        await _userRepository.CreateUserAsync(userCreate.ToUserEntity(accessTokenInfo));

        return accessTokenInfo;
    }

    public async Task ChangePasswordAsync(UserUpdate userUpdate)
    {
        _userValidator.ValidatePassword(userUpdate.NewPassword);

        await _userRepository.ChangePasswordAsync(userUpdate.Login, userUpdate.NewPassword);
    }
}