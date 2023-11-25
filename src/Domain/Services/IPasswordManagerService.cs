using Domain.Models;

namespace Domain.Services;

public interface IPasswordManagerService
{
    public Task<IEnumerable<PasswordStorage>> GetAllPasswordsAsync(string login);
}