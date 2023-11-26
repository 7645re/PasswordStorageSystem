using System.Threading.Tasks;
using Domain.DTO;

namespace Domain.Services;

public interface IUserService
{
    public Task<string> GetPasswordByLoginAsync(string login);
    public Task<OperationResult> ChangePasswordByLoginAsync(UserToChangePassword userToChangePassword);
    public Task<OperationResult> CreateUserAsync(UserToCreate userToCreate);
}