using System.Threading.Tasks;
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

    public async Task<OperationResult> ChangePasswordByLoginAsync(UserToChangePassword userToChangePassword)
    {
        if (await _userRepository.UserExistAsync(userToChangePassword.Login, userToChangePassword.NewPassword))
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "User already has such a password"
            };
        await _userRepository.ChangePasswordByLoginAsync(userToChangePassword.Login, userToChangePassword.NewPassword);
        return new OperationResult
        {
            IsSuccess = true
        };
    }
    
    public async Task<OperationResult> CreateUserAsync(UserToCreate userToCreate)
    {
        var user = await _userRepository.GetByLoginAsync(userToCreate.Login);
        if (user != null) 
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = "User already exist"
            };

        await _userRepository.CreateUserAsync(userToCreate.Login, userToCreate.Password);
        return new OperationResult
        {
            IsSuccess = true
        };
    }
}