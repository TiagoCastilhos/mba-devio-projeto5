using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Controllers;
using Coldmart.Core.Notificacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coldmart.BFF.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class CursosController : CustomControllerBase
{
    private readonly ICursoService _cursoService;

    public CursosController(ICursoService cursoService, INotificador notificador) : base(notificador)
    {
        _cursoService = cursoService;
    }

    [HttpGet()]
    public async Task<IActionResult> ObterTodos()
    {
        var response = await _cursoService.ObterTodos();
        return CustomResponse(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> CriarCursoAsync(Guid id)
    {
        var response = await _cursoService.ObterPorId(id);
        return CustomResponse(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> CriarCursoAsync([FromBody] CursoViewModel viewModel)
    {
        var response = await _cursoService.CriarCursoAsync(viewModel);
        return CustomResponse(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> EditarCursoAsync([FromRoute] Guid id, [FromBody] CursoViewModel viewModel)
    {
        viewModel.Id = id;
        var response = await _cursoService.EditarCursoAsync(viewModel);
        return CustomResponse(response);
    }

    [HttpPost("aulas")]
    public async Task<IActionResult> AdicionarAulaAsync([FromBody] AulaViewModel viewModel)
    {
        var response = await _cursoService.AdicionarAulaAsync(viewModel);
        return CustomResponse(response);
    }
}
