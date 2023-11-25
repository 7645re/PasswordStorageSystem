using Domain.DTO;

namespace Domain.Services;

public interface IUserService
{
    public Task<string> GetPasswordByLoginAsync(string login);
    public Task<bool> ChangePasswordByLoginAsync(UserToChangePassword userToChangePassword);
    public Task<bool> CreateUserAsync(UserToCreate userToCreate);
}