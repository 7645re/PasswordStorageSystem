using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByLoginAsync(string login);
    public Task<bool> UserExistAsync(string login, string password);
    public Task<string> GetPasswordByLoginAsync(string login);
    public Task ChangePasswordByLoginAsync(string login, string password);
    public Task CreateUserAsync(string login, string password);
    public Task<bool> UserExistByLoginAsync(string login);
}
