using Domain.DTO;
using Domain.Models;

namespace Domain.Services;

public interface IUserService
{
    Task<OperationResult<IEnumerable<User>>> GetAllUsersAsync();
    Task<OperationResult<User>> GetUserAsync(string userLogin);
    Task<OperationResult> DeleteUserAsync(string userLogin);
    Task<OperationResult<TokenInfo>> CreateUserAsync(UserCreate userCreate);
    Task<OperationResult> ChangePasswordAsync(string userLogin, string newPassword);
    Task<OperationResult<TokenInfo>> GetUserTokenAsync(UserSearch userSearch);
}