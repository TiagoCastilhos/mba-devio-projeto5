using Coldmart.Core.Eventos;
using Coldmart.Core.Notificacao;
using Coldmart.Pagamentos.Business.Requests;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Domain;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Pagamentos.Business.Services;

public class PagamentosService :
    IRequestHandler<CriarPagamentoRequest>,
    IRequestHandler<AprovarPagamentoRequest>,
    IRequestHandler<CancelarPagamentoRequest>
{
    private readonly IPagamentosDbContext _dbContext;
    private readonly INotificador _notificador;
    private readonly IPublishEndpoint _publishEndpoint;

    public PagamentosService(IPagamentosDbContext dbContext, INotificador notificador, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _notificador = notificador;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(CriarPagamentoRequest request, CancellationToken cancellationToken)
    {
        var matricula = await _dbContext.Matriculas.FirstOrDefaultAsync(m => m.Id == request.Pagamento.MatriculaId, cancellationToken);
        if (matricula == null)
        {
            _notificador.AdicionarErro($"Matricula '{request.Pagamento.MatriculaId}' não encontrado.");
            return;
        }

        var dadosCartao = new DadosCartao(
            request.Pagamento.Cartao.NumeroCartao,
            request.Pagamento.Cartao.NomeTitular,
            request.Pagamento.Cartao.DataValidade,
            request.Pagamento.Cartao.CodigoSeguranca);

        var novoPagamento = new Pagamento(
            dadosCartao,
            request.Pagamento.Valor,
            request.Pagamento.MatriculaId);

        await _dbContext.Pagamentos.AddAsync(novoPagamento, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(AprovarPagamentoRequest request, CancellationToken cancellationToken)
    {
        var pagamento = await _dbContext.Pagamentos.FirstOrDefaultAsync(p => p.Id == request.Pagamento.PagamentoId, cancellationToken);
        if (pagamento == null)
        {
            _notificador.AdicionarErro($"Pagamento '{request.Pagamento.PagamentoId}' não encontrado.");
            return;
        }

        pagamento.Aprovar();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new PagamentoRealizadoEvento { MatriculaId = pagamento.MatriculaId }, cancellationToken);
    }

    public async Task Handle(CancelarPagamentoRequest request, CancellationToken cancellationToken)
    {
        var pagamento = await _dbContext.Pagamentos.FirstOrDefaultAsync(p => p.Id == request.Pagamento.PagamentoId, cancellationToken);
        if (pagamento == null)
        {
            _notificador.AdicionarErro($"Pagamento '{request.Pagamento.PagamentoId}' não encontrado.");
            return;
        }

        pagamento.Cancelar();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new PagamentoCanceladoEvento { MatriculaId = pagamento.MatriculaId }, cancellationToken);
    }
}
