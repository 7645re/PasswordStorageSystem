using Domain.DTO;
using Domain.Models;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    Task<UserEntity?> GetUserAsync(string login);
    Task DeleteUserAsync(string login);
    Task CreateUserAsync(UserEntity userEntity);
    Task ChangePasswordAsync(string login, string newPassword);
    Task ChangeAccessTokenAsync(string login, TokenInfo tokenInfo);
}