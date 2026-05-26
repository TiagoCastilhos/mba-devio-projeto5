using AutoFixture;
using AutoFixture.Xunit2;
using Coldmart.Alunos.Business.Services;
using Coldmart.Alunos.Data.Contexts;
using Coldmart.Alunos.Domain;
using Coldmart.Alunos.Domain.Enumerations;
using Coldmart.Core.Eventos;
using Coldmart.Core.Tests.Attributes;
using Coldmart.Core.Tests.Extensions;
using Moq;

namespace Coldmart.Alunos.Business.Tests.Services;

public class AlunosEventosServiceTests
{
    [Theory, AutoDomainData]
    public async Task Handle_AulaRealizadaEvento_DeveGerarCertificado(
        [Frozen] Mock<IAlunosDbContext> dbContext,
        AlunosEventosService service,
        Aluno aluno, Matricula matricula,
        Curso curso,
        List<Aula> aulasCurso,
        List<HistoricoAluno> historicosAlunos,
        CancellationToken cancellationToken)
    {
        //arrange
        var evento = new AulaRealizadaEvento { AlunoId = aluno.Id, CursoId = curso.Id };

        aulasCurso =
        [
            new Aula { Id = Guid.NewGuid(), CursoId = curso.Id },
            new Aula { Id = Guid.NewGuid(), CursoId = curso.Id }
        ];

        historicosAlunos =
        [
            new HistoricoAluno(aluno.Id, aulasCurso[0].Id, curso.Id),
            new HistoricoAluno(aluno.Id, aulasCurso[1].Id, curso.Id)
        ];

        dbContext.Setup(db => db.Aulas)
            .Returns(DbSetHelper.CreateMockedDbSet(aulasCurso).Object);

        dbContext.Setup(db => db.HistoricosAlunos)
            .Returns(DbSetHelper.CreateMockedDbSet(historicosAlunos).Object);

        dbContext.Setup(db => db.Cursos)
            .Returns(DbSetHelper.CreateMockedDbSet([curso]).Object);

        dbContext.Setup(db => db.Alunos)
            .Returns(DbSetHelper.CreateMockedDbSet([aluno]).Object);

        var certificadosDbSet = DbSetHelper.CreateMockedDbSet(new List<Certificado>());
        dbContext.Setup(db => db.Certificados).Returns(certificadosDbSet.Object);

        matricula.SetProperty("AlunoId", aluno.Id);
        matricula.SetProperty("CursoId", curso.Id);

        dbContext.Setup(db => db.Matriculas)
            .Returns(DbSetHelper.CreateMockedDbSet([matricula]).Object);

        //act
        await service.Handle(evento, cancellationToken);

        //assert
        certificadosDbSet.Verify(c => c.AddAsync(It.IsAny<Certificado>(), cancellationToken), Times.Once);
        dbContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusMatricula.Concluido, matricula.Status);
    }

    [Theory, AutoDomainData]
    public async Task Handle_AulaRealizadaEvento_QuandoFaltamAulasASeremRealizadas_NaoDeveGerarCertificado(
       [Frozen] Mock<IAlunosDbContext> dbContext,
       AlunosEventosService service,
       Aluno aluno,
       Curso curso,
       List<Aula> aulasCurso,
       List<HistoricoAluno> historicosAlunos,
       CancellationToken cancellationToken)
    {
        //arrange
        var evento = new AulaRealizadaEvento { AlunoId = aluno.Id, CursoId = curso.Id };

        aulasCurso =
        [
            new Aula { Id = Guid.NewGuid(), CursoId = curso.Id },
            new Aula { Id = Guid.NewGuid(), CursoId = curso.Id }
        ];

        historicosAlunos =
        [
            new HistoricoAluno(aluno.Id, aulasCurso[0].Id, curso.Id)
        ];

        dbContext.Setup(db => db.Aulas)
            .Returns(DbSetHelper.CreateMockedDbSet(aulasCurso).Object);

        dbContext.Setup(db => db.HistoricosAlunos)
            .Returns(DbSetHelper.CreateMockedDbSet(historicosAlunos).Object);

        dbContext.Setup(db => db.Cursos)
            .Returns(DbSetHelper.CreateMockedDbSet([curso]).Object);

        dbContext.Setup(db => db.Alunos)
            .Returns(DbSetHelper.CreateMockedDbSet([aluno]).Object);

        var certificadosDbSet = DbSetHelper.CreateMockedDbSet(new List<Certificado>());
        dbContext.Setup(db => db.Certificados).Returns(certificadosDbSet.Object);

        //act
        await service.Handle(evento, cancellationToken);

        //assert
        certificadosDbSet.Verify(c => c.AddAsync(It.IsAny<Certificado>(), cancellationToken), Times.Never);
        dbContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Never);
    }

    [Theory, AutoDomainData]
    public async Task Handle_PagamentoRealizadoEvento_DeveIniciarMatricula(
       [Frozen] Mock<IAlunosDbContext> dbContext,
       AlunosEventosService service,
       Matricula matricula,
       CancellationToken cancellationToken)
    {
        //arrange
        var evento = new PagamentoRealizadoEvento { MatriculaId = matricula.Id };
        dbContext.Setup(db => db.Matriculas)
            .Returns(DbSetHelper.CreateMockedDbSet([matricula]).Object);

        //act
        await service.Handle(evento, cancellationToken);

        //assert
        dbContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusMatricula.Iniciado, matricula.Status);
    }

    [Theory, AutoDomainData]
    public async Task Handle_PagamentoCanceladoEvento_DeveCancelarMatricula(
       [Frozen] Mock<IAlunosDbContext> dbContext,
       AlunosEventosService service,
       Matricula matricula,
       CancellationToken cancellationToken)
    {
        //arrange
        var evento = new PagamentoCanceladoEvento { MatriculaId = matricula.Id };
        dbContext.Setup(db => db.Matriculas)
            .Returns(DbSetHelper.CreateMockedDbSet([matricula]).Object);

        //act
        await service.Handle(evento, cancellationToken);

        //assert
        dbContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusMatricula.Cancelado, matricula.Status);
    }
}
