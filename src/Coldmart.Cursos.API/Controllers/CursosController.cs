using Coldmart.Core.Constants;
using Coldmart.Core.Controllers;
using Coldmart.Core.Notificacao;
using Coldmart.Cursos.Business.Requests;
using Coldmart.Cursos.Business.Services;
using Coldmart.Cursos.Business.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coldmart.Cursos.API.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class CursosController : CustomControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICursoQueries _cursoQueries;

    public CursosController(IMediator mediator, ICursoQueries cursoQueries, INotificador notificador)
        : base(notificador)
    {
        _mediator = mediator;
        _cursoQueries = cursoQueries;
    }

    [HttpGet]
    [Authorize(Roles = RolesConstants.Admin + "," + RolesConstants.Aluno)]
    public async Task<IActionResult> ObterTodosCursoAsync()
    {
        return CustomResponse(await _cursoQueries.ObterTodos());
    }

    [HttpGet("{id:Guid}")]
    [Authorize(Roles = RolesConstants.Admin + "," + RolesConstants.Aluno)]
    public async Task<IActionResult> ObterPorIdCursoAsync(Guid id)
    {
        var result = await _cursoQueries.ObterPorId(id);
        return CustomResponse(result);
    }

    [HttpPost("")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> CriarCursoAsync([FromBody] CursoViewModel viewModel)
    {
        await _mediator.Send(new CriarCursoRequest
        {
            Curso = viewModel
        }, HttpContext.RequestAborted);

        return CustomResponse();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> EditarCursoAsync([FromRoute] Guid id, [FromBody] CursoViewModel viewModel)
    {
        viewModel.Id = id;
        await _mediator.Send(new EditarCursoRequest
        {
            Curso = viewModel
        }, HttpContext.RequestAborted);
        return CustomResponse();
    }

    [HttpPost("{id:guid}/aulas")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> AdicionarAulaAsync([FromRoute] Guid id, [FromBody] AulaViewModel viewModel)
    {
        await _mediator.Send(new AdicionarAulaRequest
        {
            Aula = viewModel
        }, HttpContext.RequestAborted);
        return CustomResponse();
    }
}