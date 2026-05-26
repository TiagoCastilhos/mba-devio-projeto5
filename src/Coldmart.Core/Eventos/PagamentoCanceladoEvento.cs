using MediatR;

namespace Coldmart.Core.Eventos;

public class PagamentoCanceladoEvento : INotification
{
    public Guid MatriculaId { get; init; }
}
