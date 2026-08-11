using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Controllers;
using Coldmart.Core.Notificacao;
using Microsoft.AspNetCore.Mvc;

namespace Coldmart.BFF.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : CustomControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, INotificador notificador) : base(notificador)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Logar([FromBody] LogarViewModel viewModel)
    {
        var response = await _authService.Logar(viewModel);
        return CustomResponse(response);
    }

    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastro([FromBody] CadastrarViewModel viewModel)
    {
        var response = await _authService.Cadastro(viewModel);
        return CustomResponse(response);
    }
}
