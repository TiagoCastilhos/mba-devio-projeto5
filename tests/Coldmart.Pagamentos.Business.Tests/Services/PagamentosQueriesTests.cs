using AutoFixture.Xunit2;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Tests.Attributes;
using Coldmart.Core.Tests.Extensions;
using Coldmart.Pagamentos.Business.Services;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Domain;
using Moq;

namespace Coldmart.Pagamentos.Business.Tests.Services
{
    public class PagamentosQueriesTests
    {
        [Theory, AutoDomainData]
        internal async Task PagamentoQueries_ObterTodosAsync_DeveRetornarPagamentos(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        PagamentoQueries service,
        List<Pagamento> pagamentos)
        {
            //arrange
            var cursosDbSet = DbSetHelper.CreateMockedDbSet(pagamentos);
            dbContext.Setup(db => db.Pagamentos).Returns(cursosDbSet.Object);

            //act
            var result = await service.ObterTodosAsync();

            //assert
            notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
            Assert.True(result.Any());
        }

        [Theory, AutoDomainData]
        internal async Task PagamentoQueries_ObterPorIdAsync_DeveRetornarPagamento(
            [Frozen] Mock<IPagamentosDbContext> dbContext,
            [Frozen] Mock<INotificador> notificador,
            PagamentoQueries service,
            List<Pagamento> pagamentos)
        {
            //arrange
            var cursosDbSet = DbSetHelper.CreateMockedDbSet(pagamentos);
            dbContext.Setup(db => db.Pagamentos).Returns(cursosDbSet.Object);

            cursosDbSet.Setup(x => x.FindAsync(cursosDbSet.Object.First().Id))
               .ReturnsAsync((pagamentos.First()));

            //act
            var result = await service.ObterPorIdAsync(pagamentos.First().Id);

            //assert
            notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
            dbContext.Verify(d => d.Pagamentos.FindAsync(It.IsAny<Guid>()), Times.Once);
            Assert.True(pagamentos.First().Id == result.Id);
        }
    }
}
