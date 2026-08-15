using AutoFixture.Xunit2;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Tests.Attributes;
using Coldmart.Core.Tests.Extensions;
using Coldmart.Cursos.Business.Services;
using Coldmart.Cursos.Data.Contexts;
using Coldmart.Cursos.Domain;
using Moq;

namespace Coldmart.Cursos.Business.Tests.Services;

public class CursosQueriesTests
{
    [Theory, AutoDomainData]
    internal async Task CursoAppService_ObterTodos_DeveRetornarCursos(
        [Frozen] Mock<ICursosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        CursoQueries service,
        List<Curso> cursos,
        List<ConteudoProgramatico> conteudosProgramaticos)
    {
        //arrange
        var cursosDbSet = DbSetHelper.CreateMockedDbSet(cursos);
        dbContext.Setup(db => db.Cursos).Returns(cursosDbSet.Object);

        var conteudosProgramaticosDbSet = DbSetHelper.CreateMockedDbSet(conteudosProgramaticos);
        dbContext.Setup(db => db.ConteudosProgramaticos).Returns(conteudosProgramaticosDbSet.Object);

        //act
        var result = await service.ObterTodos();

        //assert
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
        Assert.True(result.Any());
    }

    [Theory, AutoDomainData]
    internal async Task CursoAppService_ObterPorId_DeveRetornarCurso(
        [Frozen] Mock<ICursosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        CursoQueries service,
        List<Curso> cursos, List<Aula> aulas)
    {
        //arrange
        var cursosDbSet = DbSetHelper.CreateMockedDbSet(cursos);
        dbContext.Setup(db => db.Cursos).Returns(cursosDbSet.Object);

        var aulasDbSet = DbSetHelper.CreateMockedDbSet(aulas);
        dbContext.Setup(db => db.Aulas).Returns(aulasDbSet.Object);

        cursosDbSet.Setup(x => x.FindAsync(cursosDbSet.Object.First().Id))
           .ReturnsAsync((cursos.First()));

        //act
        var result = await service.ObterPorId(cursos.First().Id);

        //assert
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
        dbContext.Verify(d => d.Cursos.FindAsync(It.IsAny<Guid>()), Times.Once);
        Assert.True(cursos.First().Id == result.Id);
    }
}
