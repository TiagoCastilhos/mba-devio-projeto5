using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Coldmart.Core.Contexts;

public class UsuarioContext : IUsuarioContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? ObterIdUsuario()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null)
            return null;

        var idClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (idClaim == null)
            return null;

        return Guid.Parse(idClaim);
    }

    public string ObterEmailUsuario()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null)
            return null;

        var emailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        return emailClaim;
    }

    public HttpContext ObterHttpContext()
    {
        return _httpContextAccessor.HttpContext;
    }

    public string ObterUserToken()
    {
        return EstaAutenticado() ? _httpContextAccessor.HttpContext.User.GetUserToken() : "";
    }

    public bool EstaAutenticado()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}