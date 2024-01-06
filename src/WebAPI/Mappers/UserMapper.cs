using Domain.DTO.User;
using WebAPI.DTO.Request;

namespace WebAPI.Mappers;

public static class UserMapper
{
    public static UserCreate ToUserCreate(this UserRegisterRequest userRegisterRequest)
    {
        return new UserCreate(userRegisterRequest.Login, userRegisterRequest.Password);
    }

    public static UserUpdate ToUserChangePassword(this UserChangePasswordRequest userChangePasswordRequest)
    {
        return new UserUpdate(userChangePasswordRequest.Login, userChangePasswordRequest.NewPassword);
    }

    public static UserLogIn ToUserLogIn(this UserLogInRequest userLogInRequest)
    {
        return new UserLogIn(userLogInRequest.Login, userLogInRequest.Password);
    }
}