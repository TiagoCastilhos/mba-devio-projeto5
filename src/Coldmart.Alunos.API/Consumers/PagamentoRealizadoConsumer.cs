using Coldmart.Core.Eventos;
using MassTransit;

namespace Coldmart.Alunos.API.Consumers;

public class PagamentoRealizadoConsumer : IConsumer<PagamentoRealizadoEvento>
{
    private readonly MediatR.IMediator _mediator;

    public PagamentoRealizadoConsumer(MediatR.IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PagamentoRealizadoEvento> context)
    {
        await _mediator.Publish(context.Message);
    }
}
