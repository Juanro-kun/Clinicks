using System.Security.Claims;
using Clinicks.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Clinicks.API;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentUserId()
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        var claimVal = claimsPrincipal?.FindFirst("UsuarioId")?.Value;
        if (int.TryParse(claimVal, out int parsedId))
        {
            return parsedId;
        }
        return null;
    }
}
