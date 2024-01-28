using Domain.DTO.User;
using Domain.Models;
using WebAPI.DTO.Request;
using WebAPI.DTO.Response;

namespace WebAPI.Mappers;

public static class UserMapper
{
    public static UserCreate ToUserCreate(this UserRegisterRequest userRegisterRequest)
    {
        return new UserCreate(
            userRegisterRequest.Login,
            userRegisterRequest.Password);
    }

    public static UserUpdate ToUserChangePassword(
        this UserChangePasswordRequest userChangePasswordRequest,
        string login)
    {
        return new UserUpdate(
            login,
            userChangePasswordRequest.NewPassword);
    }

    public static UserLogIn ToUserLogIn(this UserLogInRequest userLogInRequest)
    {
        return new UserLogIn(
            userLogInRequest.Login,
            userLogInRequest.Password);
    }

    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            Login = user.Login,
            Password = user.Password,
            Token = user.Token,
            TokenExpire = user.TokenExpire
        };
    }
}