using MediatR;

namespace Coldmart.Core.Eventos;

public class AulaRealizadaEvento : INotification
{
    public Guid AlunoId { get; init; }
    public Guid AulaId { get; init; }
    public Guid CursoId { get; init; }
}
