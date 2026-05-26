using System.Diagnostics.CodeAnalysis;
using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Controllers;
using Coldmart.Core.Notificacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coldmart.BFF.Controllers;

[ExcludeFromCodeCoverage]
[ApiController, Authorize]
[Route("api/[controller]")]
public class AlunosController : CustomControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunosController(IAlunoService alunoService, INotificador notificador) : base(notificador)
    {
        _alunoService = alunoService;
    }

    [HttpPost("cursos")]
    public async Task<IActionResult> MatricularAoCurso([FromBody] MatriculaViewModel viewModel)
    {
        var response = await _alunoService.MatricularAoCurso(viewModel);
        return CustomResponse(response);
    }

    [HttpPut("cursos/aulas")]
    public async Task<IActionResult> RealizarAula([FromBody] RealizarAulaViewModel viewModel)
    {
        var response = await _alunoService.RealizarAula(viewModel);
        return CustomResponse(response);
    }

    [HttpGet("historico")]
    public async Task<IActionResult> Historico()
    {
        var response = await _alunoService.Historico();
        return CustomResponse(response);
    }

    [HttpPost("finalizar")]
    public async Task<IActionResult> Finalizar([FromBody] FinalizarViewModel viewModel)
    {
        var response = await _alunoService.Finalizar(viewModel);
        return CustomResponse(response);
    }

    [HttpGet("certificado/{id:guid}")]
    public async Task<IActionResult> Certificado([FromRoute] Guid id)
    {
        var response = await _alunoService.Certificado(id);
        return CustomResponse(response);
    }
}
