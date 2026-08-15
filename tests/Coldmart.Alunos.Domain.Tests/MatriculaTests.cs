using Coldmart.Alunos.Domain.Enumerations;
using Coldmart.Core.Tests.Attributes;

namespace Coldmart.Alunos.Domain.Tests;

public class MatriculaTests
{
    [Theory, AutoDomainData]
    public void CriarMatricula_FornecidoCurso_DeveCriar(Curso curso, Aluno aluno)
    {
        //act
        var matricula = new Matricula(curso, aluno);

        //assert
        Assert.Equal(curso, matricula.Curso);
        Assert.Equal(aluno, matricula.Aluno);
        Assert.Equal(StatusMatricula.AguardandoPagamento, matricula.Status);
        Assert.NotEqual(DateTimeOffset.MinValue, matricula.DataAtualizacao);
    }

    [Theory, AutoDomainData]
    public void CriarMatricula_CursoNulo_DeveLancarExcecao(Aluno aluno)
    {
        //act & assert
        Assert.Throws<ArgumentNullException>(() => new Matricula(null!, aluno));
    }

    [Theory, AutoDomainData]
    public void CriarMatricula_AlunoNulo_DeveLancarExcecao(Curso curso)
    {
        //act & assert
        Assert.Throws<ArgumentNullException>(() => new Matricula(curso, null!));
    }
}
