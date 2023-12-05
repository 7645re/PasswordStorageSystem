using Domain.DTO;

namespace Domain.Services;

public interface IUserService
{
    Task<OperationResult<User>> GetUserAsync(string userLogin);
    Task<OperationResult> DeleteUserAsync(string userLogin);
    Task<OperationResult<TokenInfo>> CreateUserAsync(UserCreate userCreate);
    Task<OperationResult> ChangePasswordAsync(UserChangePassword userChangePassword);
    Task<OperationResult<TokenInfo>> GetUserTokenAsync(UserLogIn userLogIn);
}