using Domain.DTO;
using Domain.Models;

namespace Domain.Repositories.UserRepository;

public interface IUserRepository
{
    Task<UserEntity?> TryGetUserAsync(string login);
    Task CheckExistAsync(string login);
    Task DeleteUserAsync(string login);
    Task CreateUserAsync(UserEntity userEntity);
    Task ChangePasswordAsync(string login, string newPassword);
    Task ChangeAccessTokenAsync(string login, TokenInfo tokenInfo);
    Task<UserEntity> GetUserAsync(string login);
}