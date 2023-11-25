using Domain.DTO;
using Domain.Models;
using Domain.Repositories;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GetPasswordByLoginAsync(string login)
    {
        return await _userRepository.GetPasswordByLoginAsync(login);
    }

    public async Task<bool> ChangePasswordByLoginAsync(UserToChangePassword userToChangePassword)
    {
        if (await _userRepository.ExistsIsUserAsync(new User(userToChangePassword.Login,
                userToChangePassword.NewPassword)))
            return false;
        await _userRepository.ChangePasswordByLoginAsync(new User(userToChangePassword.Login,
            userToChangePassword.NewPassword));
        return true;
    }

    public async Task<bool> CreateUserAsync(UserToCreate userToCreate)
    {
        if (await _userRepository.GetByLoginAsync(userToCreate.Login) != null ||
            await _userRepository.ExistsIsUserAsync(new User(userToCreate.Login, userToCreate.Password))) return false;
        await _userRepository.CreateUserAsync(new User(userToCreate.Login, userToCreate.Password));
        return true;
    }
}