using MediatR;

namespace Coldmart.Core.Eventos;

public class MatriculaRealizadaEvento : INotification
{
    public Guid MatriculaId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
}
