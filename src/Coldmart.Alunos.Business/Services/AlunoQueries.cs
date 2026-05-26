using Coldmart.Alunos.Data.Contexts;
using Coldmart.Alunos.Domain;
using Coldmart.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Alunos.Business.Services;

public class AlunoQueries : IAlunoQueries
{
    private readonly IAlunosDbContext _dbContext;
    private readonly IUsuarioContext _usuarioContext;

    public AlunoQueries(IAlunosDbContext dbContext, IUsuarioContext usuarioContext)
    {
        _dbContext = dbContext;
        _usuarioContext = usuarioContext;
    }

    public async Task<IEnumerable<HistoricoAluno>> Historico()
    {
        var usuarioId = _usuarioContext.ObterIdUsuario();
        var aluno = await _dbContext.Alunos.FirstOrDefaultAsync(a => a.Id == usuarioId);
        return aluno?.Historicos ?? [];
    }

    public async Task<Certificado> Certificado(Guid id)
    {
        var usuarioId = _usuarioContext.ObterIdUsuario();
        var aluno = await _dbContext.Alunos.FirstOrDefaultAsync(a => a.Id == usuarioId);
        return aluno?.Certificados?.FirstOrDefault(c => c.Id == id);
    }
}
