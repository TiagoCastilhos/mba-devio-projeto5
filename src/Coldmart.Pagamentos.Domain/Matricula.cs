using Coldmart.Core.Domain;

namespace Coldmart.Pagamentos.Domain;

public class Matricula : Entity
{
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }

    public Matricula() { }

    public Matricula(Guid matriculaId, Guid alunoId, Guid cursoId) : base(matriculaId)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
    }
}
