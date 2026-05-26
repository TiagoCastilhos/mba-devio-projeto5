using Coldmart.Alunos.Business.Requests;
using Coldmart.Alunos.Data.Contexts;
using Coldmart.Alunos.Domain;
using Coldmart.Core.Contexts;
using Coldmart.Core.Eventos;
using Coldmart.Core.Notificacao;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Alunos.Business.Services;

public class AlunosService : IRequestHandler<MatricularAoCursoRequest>, IRequestHandler<RealizarAulaRequest>, IRequestHandler<FinalizarRequest>
{
    private readonly IAlunosDbContext _dbContext;
    private readonly IUsuarioContext _usuarioContext;
    private readonly INotificador _notificador;
    private readonly IPublishEndpoint _publishEndpoint;

    public AlunosService(IAlunosDbContext dbContext, IUsuarioContext usuarioContext, INotificador notificador, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _usuarioContext = usuarioContext;
        _notificador = notificador;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(MatricularAoCursoRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = _usuarioContext.ObterIdUsuario();

        var aluno = await _dbContext.Alunos.FirstOrDefaultAsync(a => a.Id == usuarioId, cancellationToken);

        aluno ??= await AdicionarAlunoAsync(usuarioId, cancellationToken);

        var curso = await _dbContext.Cursos.FirstOrDefaultAsync(c => c.Id == request.Matricula.CursoId, cancellationToken);
        if (curso == null)
        {
            _notificador.AdicionarErro($"Curso '{request.Matricula.CursoId}' não encontrado.");
            return;
        }

        var matricula = new Matricula(curso, aluno);
        await _dbContext.Matriculas.AddAsync(matricula, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _publishEndpoint.Publish(new MatriculaRealizadaEvento { MatriculaId = matricula.Id, AlunoId = aluno.Id, CursoId = curso.Id }, cancellationToken);
    }

    public async Task Handle(RealizarAulaRequest request, CancellationToken cancellationToken)
    {
        var alunoId = _usuarioContext.ObterIdUsuario();

        var aluno = await _dbContext.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId, cancellationToken);
        if (aluno == null)
        {
            _notificador.AdicionarErro($"Aluno '{alunoId}' não encontrado.");
            return;
        }

        var aula = await _dbContext.Aulas.FirstOrDefaultAsync(a => a.Id == request.Aula.AulaId, cancellationToken);
        if (aula == null)
        {
            _notificador.AdicionarErro($"Aula '{request.Aula.AulaId}' não encontrada.");
            return;
        }

        var historicoAluno = new HistoricoAluno(aluno.Id, aula.Id, aula.CursoId);
        await _dbContext.HistoricosAlunos.AddAsync(historicoAluno, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new AulaRealizadaEvento { AlunoId = aluno.Id, AulaId = aula.Id, CursoId = aula.CursoId }, cancellationToken);
    }

    public async Task Handle(FinalizarRequest request, CancellationToken cancellationToken)
    {
        var alunoId = _usuarioContext.ObterIdUsuario();
        var aluno = await _dbContext.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId, cancellationToken);
        if (aluno == null)
        {
            _notificador.AdicionarErro($"Aluno '{alunoId}' não encontrado.");
            return;
        }

        var matricula = aluno.Matriculas.FirstOrDefault(m => m.CursoId == request.Matricula.CursoId);
        if (matricula == null)
        {
            _notificador.AdicionarErro($"Matrícula para o curso '{request.Matricula.CursoId}' não encontrada.");
            return;
        }

        matricula.Concluir();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Aluno> AdicionarAlunoAsync(Guid? usuarioId, CancellationToken cancellationToken)
    {
        var usuarioEmail = _usuarioContext.ObterEmailUsuario();

        var aluno = new Aluno(usuarioId!.Value, usuarioEmail, usuarioEmail);

        await _dbContext.Alunos.AddAsync(aluno, cancellationToken);

        return aluno;
    }
}
