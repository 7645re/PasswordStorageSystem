using Domain.DTO;
using Domain.Models;

namespace Domain.Mappers;

public static class UserMapper
{
    public static User ToUser(this UserEntity userEntity)
    {
        return new User
        {
            Login = userEntity.Login,
            Password = userEntity.Password,
            Token = userEntity.Token,
            TokenExpire = userEntity.TokenExpire,
        };
    }
    
    public static UserEntity ToUserEntity(
        this UserCreate userCreate,
        string token,
        DateTimeOffset tokenExpire)
    {
        return new UserEntity
        {
            Login = userCreate.Login,
            Password = userCreate.Password,
            Token = token,
            TokenExpire = tokenExpire
        };
    }
}