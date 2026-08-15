using AutoFixture.Xunit2;
using Coldmart.Core.Eventos;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Tests.Attributes;
using Coldmart.Core.Tests.Extensions;
using Coldmart.Pagamentos.Business.Requests;
using Coldmart.Pagamentos.Business.Services;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Domain;
using MassTransit;
using MediatR;
using Moq;

namespace Coldmart.Pagamentos.Business.Tests.Services;

public class PagamentosServiceTests
{
    [Theory, AutoDomainData]
    public async Task HandleCriarPagamento_FornecidoDadosPagamento_DeveCriar(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        Matricula matricula, List<Pagamento> pagamentos,
        PagamentosService pagamentosService,
        CriarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        request.Pagamento.MatriculaId = matricula.Id;
        dbContext.Setup(db => db.Matriculas).Returns(DbSetHelper.CreateMockedDbSet([matricula]).Object);

        var pagamentosDbSet = DbSetHelper.CreateMockedDbSet(pagamentos);
        dbContext.Setup(db => db.Pagamentos).Returns(pagamentosDbSet.Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        pagamentosDbSet.Verify(m => m.AddAsync(It.Is<Pagamento>(
            p => p.MatriculaId == request.Pagamento.MatriculaId && p.Valor == request.Pagamento.Valor), It.IsAny<CancellationToken>())
        , Times.Once);
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);

    }

    [Theory, AutoDomainData]
    public async Task HandleCriarPagamento_FornecidoDadosPagamento_MatriculaNaoExiste_DeveAdicionarErro(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        List<Matricula> matriculas, List<Pagamento> pagamentos,
        PagamentosService pagamentosService,
        CriarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        dbContext.Setup(db => db.Matriculas).Returns(DbSetHelper.CreateMockedDbSet(matriculas).Object);

        var pagamentosDbSet = DbSetHelper.CreateMockedDbSet(pagamentos);
        dbContext.Setup(db => db.Pagamentos).Returns(pagamentosDbSet.Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        pagamentosDbSet.Verify(m => m.AddAsync(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()), Times.Never);
        notificador.Verify(n => n.AdicionarErro(It.Is<string>(s => s.Contains(request.Pagamento.MatriculaId.ToString()))), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task HandleAprovarPagamento_FornecidoPagamento_DeveAtualizar(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        [Frozen] Mock<IPublishEndpoint> publishEndpoint,
        Pagamento pagamento,
        PagamentosService pagamentosService,
        AprovarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        request.Pagamento.PagamentoId = pagamento.Id;
        dbContext.Setup(db => db.Pagamentos).Returns(DbSetHelper.CreateMockedDbSet([pagamento]).Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
        Assert.Equal(StatusPagamento.Aprovado, pagamento.Status);
        publishEndpoint.Verify(m => m.Publish(It.IsAny<PagamentoRealizadoEvento>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task HandleAprovarPagamento_PagamentoNaoEncontrado_DeveAdicionarErro(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        [Frozen] Mock<IMediator> mediator,
        Pagamento pagamento,
        PagamentosService pagamentosService,
        AprovarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        dbContext.Setup(db => db.Pagamentos).Returns(DbSetHelper.CreateMockedDbSet([pagamento]).Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        notificador.Verify(n => n.AdicionarErro(It.Is<string>(s => s.Contains(request.Pagamento.PagamentoId.ToString()))), Times.Once);
        mediator.Verify(m => m.Publish(It.IsAny<PagamentoRealizadoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory, AutoDomainData]
    public async Task HandleCancelarPagamento_FornecidoPagamento_DeveAtualizar(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        [Frozen] Mock<IPublishEndpoint> publishEndpoint,
        Pagamento pagamento,
        PagamentosService pagamentosService,
        CancelarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        request.Pagamento.PagamentoId = pagamento.Id;
        dbContext.Setup(db => db.Pagamentos).Returns(DbSetHelper.CreateMockedDbSet([pagamento]).Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
        Assert.Equal(StatusPagamento.Cancelado, pagamento.Status);
        publishEndpoint.Verify(m => m.Publish(It.IsAny<PagamentoCanceladoEvento>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task HandleCancelarPagamento_PagamentoNaoEncontrado_DeveAdicionarErro(
        [Frozen] Mock<IPagamentosDbContext> dbContext,
        [Frozen] Mock<INotificador> notificador,
        [Frozen] Mock<IMediator> mediator,
        Pagamento pagamento,
        PagamentosService pagamentosService,
        CancelarPagamentoRequest request,
        CancellationToken cancellationToken)
    {
        //arrange
        dbContext.Setup(db => db.Pagamentos).Returns(DbSetHelper.CreateMockedDbSet([pagamento]).Object);

        //act
        await pagamentosService.Handle(request, cancellationToken);

        //assert
        dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        notificador.Verify(n => n.AdicionarErro(It.Is<string>(s => s.Contains(request.Pagamento.PagamentoId.ToString()))), Times.Once);
        mediator.Verify(m => m.Publish(It.IsAny<PagamentoCanceladoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
