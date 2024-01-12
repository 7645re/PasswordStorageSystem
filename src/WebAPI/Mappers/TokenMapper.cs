using Domain.DTO;
using WebAPI.DTO.Response;

namespace WebAPI.Mappers;

public static class TokenMapper
{
    public static TokenInfoResponse ToTokenInfoResponse(this TokenInfo tokenInfo)
    {
        return new TokenInfoResponse
        {
            Token = tokenInfo.Token,
            Expire = tokenInfo.Expire
        };
    }
}