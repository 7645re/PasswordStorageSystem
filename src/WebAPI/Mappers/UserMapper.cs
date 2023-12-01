using Domain.DTO;
using WebAPI.DTO.Request;

namespace WebAPI.Mappers;

public static class UserMapper
{
    public static UserCreate ToUserCreate(this UserRegisterRequest userRegisterRequest)
    {
        return new UserCreate(userRegisterRequest.Login, userRegisterRequest.Password);
    }

    public static UserChangePassword ToUserChangePassword(this UserChangePasswordRequest userChangePasswordRequest)
    {
        return new UserChangePassword(userChangePasswordRequest.Login, userChangePasswordRequest.NewPassword);
    }
    
    public static UserLogIn ToUserLogIn(this UserLogInRequest userLogInRequest)
    {
        return new UserLogIn(userLogInRequest.Login, userLogInRequest.Password);
    }
}