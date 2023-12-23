using Domain.DTO;

namespace Domain.Services.UserService;

public interface IUserService
{
    Task<TokenInfo> GetUserTokenAsync(UserLogIn userLogIn);
    Task<User> GetUserAsync(string userLogin);
    Task DeleteUserAsync(string userLogin);
    Task<TokenInfo> CreateUserAsync(UserCreate userCreate);
    Task ChangePasswordAsync(UserChangePassword userChangePassword);
}