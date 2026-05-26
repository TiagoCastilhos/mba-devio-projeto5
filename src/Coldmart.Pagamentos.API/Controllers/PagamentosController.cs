using Coldmart.Core.Constants;
using Coldmart.Core.Controllers;
using Coldmart.Core.Notificacao;
using Coldmart.Pagamentos.Business.Requests;
using Coldmart.Pagamentos.Business.Services;
using Coldmart.Pagamentos.Business.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coldmart.Pagamentos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController : CustomControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPagamentoQueries _pagamentoQueries;

    public PagamentosController(IMediator mediator, IPagamentoQueries pagamentoQueries, INotificador notificador)
        : base(notificador)
    {
        _mediator = mediator;
        _pagamentoQueries = pagamentoQueries;
    }

    [HttpGet()]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> ObterTodosAync()
    {
        var pagamentos = await _pagamentoQueries.ObterTodosAsync();
        return CustomResponse(pagamentos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> ObterPorIdAsync([FromRoute] Guid id)
    {
        var pagamento = await _pagamentoQueries.ObterPorIdAsync(id);
        return CustomResponse(pagamento);
    }

    [HttpPost("")]
    [Authorize(Roles = RolesConstants.Aluno)]
    public async Task<IActionResult> CriarPagamentoAsync([FromBody] PagamentoViewModel pagamento)
    {
        await _mediator.Send(new CriarPagamentoRequest()
        {
            Pagamento = pagamento
        }, HttpContext.RequestAborted);

        return CustomResponse();
    }

    [HttpPut("{id:guid}/aprovar")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> AprovarPagamentoAsync([FromRoute] Guid id)
    {
        await _mediator.Send(new AprovarPagamentoRequest
        {
            Pagamento = new AlterarStatusPagamentoViewModel { PagamentoId = id }
        }, HttpContext.RequestAborted);

        return CustomResponse();
    }

    [HttpPut("{id:guid}/cancelar")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<IActionResult> CancelarPagamentoAsync([FromRoute] Guid id)
    {
        await _mediator.Send(new CancelarPagamentoRequest
        {
            Pagamento = new AlterarStatusPagamentoViewModel { PagamentoId = id }
        }, HttpContext.RequestAborted);

        return CustomResponse();
    }
}
