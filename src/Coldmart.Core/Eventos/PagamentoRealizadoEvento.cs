using MediatR;

namespace Coldmart.Core.Eventos;

public class PagamentoRealizadoEvento : INotification
{
    public Guid MatriculaId { get; init; }
}
