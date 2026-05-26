using Coldmart.Core.Eventos;
using MassTransit;

namespace Coldmart.Alunos.API.Consumers;

public class PagamentoCanceladoConsumer : IConsumer<PagamentoCanceladoEvento>
{
    private readonly MediatR.IMediator _mediatr;

    public PagamentoCanceladoConsumer(MediatR.IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<PagamentoCanceladoEvento> context)
    {
        await _mediatr.Publish(context.Message);
    }
}
