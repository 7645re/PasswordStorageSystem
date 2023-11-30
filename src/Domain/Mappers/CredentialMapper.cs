using Domain.DTO;
using Domain.Models;

namespace Domain.Mappers;

public static class CredentialMapper
{
    public static User ToUser(this UserEntity userEntity)
    {
        return new User
        {
            Login = userEntity.Login,
            Password = userEntity.Password
        };
    }

    
    // public static TokenInfo ToTokenInfo(this UserEntity userEntity)
    // {
    //     return new TokenInfo()
    // }
}