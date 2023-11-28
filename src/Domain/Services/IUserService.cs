using Domain.DTO;
using Domain.Models;

namespace Domain.Services;

public interface IUserService
{
    Task<OperationResult<IEnumerable<UserEntity>>> GetAllUsersAsync();
    Task<OperationResult<UserEntity>> GetUserAsync(string userLogin);
    Task<OperationResult> DeleteUserAsync(string userLogin);
    Task<OperationResult<TokenInfo>> CreateUserAsync(string userLogin, string password);
    Task<OperationResult> ChangePasswordAsync(string userLogin, string password);
    Task<OperationResult<TokenInfo>> GetUserByLoginAndPasswordAsync(string userLogin, string password);
}