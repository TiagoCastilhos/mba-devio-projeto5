using Coldmart.Core.Eventos;
using MassTransit;

namespace Coldmart.Alunos.API.Consumers;

public class AulaRealizadaConsumer : IConsumer<AulaRealizadaEvento>
{
    private readonly MediatR.IMediator _mediator;

    public AulaRealizadaConsumer(MediatR.IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<AulaRealizadaEvento> context)
    {
        await _mediator.Publish(context.Message);
    }
}
