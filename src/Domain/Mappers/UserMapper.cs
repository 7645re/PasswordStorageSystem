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
            Token = userEntity.AccessToken,
            TokenExpire = userEntity.AccessTokenExpire,
        };
    }
    
    public static UserEntity ToUserEntity(
        this UserCreate userCreate,
        TokenInfo accessToken)
    {
        return new UserEntity
        {
            Login = userCreate.Login,
            Password = userCreate.Password,
            AccessToken = accessToken.Token,
            AccessTokenExpire = accessToken.Expire
        };
    }
}