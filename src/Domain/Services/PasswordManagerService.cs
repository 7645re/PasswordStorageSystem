using Domain.Models;
using Domain.Repositories;

namespace Domain.Services;

public class PasswordManagerService : IPasswordManagerService
{
    private readonly IPasswordStorageRepository _passwordStorageRepository;

    public PasswordManagerService(IPasswordStorageRepository passwordStorageRepository)
    {
        _passwordStorageRepository = passwordStorageRepository;
    }

    public async Task<IEnumerable<PasswordStorage>> GetAllPasswordsAsync(string login)
    {
        return await _passwordStorageRepository.GetAllPasswordsByLoginAsync(login);
    }
}