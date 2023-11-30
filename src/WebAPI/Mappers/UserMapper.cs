using Domain.DTO;
using WebAPI.DTO.Request;

namespace WebAPI.Mappers;

public static class UserMapper
{
    public static UserCreate ToUserCreate(this UserCreateRequest userCreateRequest)
    {
        return new UserCreate(userCreateRequest.Login, userCreateRequest.Password);
    }
    
    public static UserSearch ToUserCreate(this UserSearchRequest userSearchRequest)
    {
        return new UserSearch(userSearchRequest.Login, userSearchRequest.Password);
    }
}