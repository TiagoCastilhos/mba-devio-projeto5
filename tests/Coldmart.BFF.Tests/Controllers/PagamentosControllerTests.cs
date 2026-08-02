using AutoFixture.Xunit2;
using Coldmart.BFF.Controllers;
using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Communication;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Tests.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Coldmart.BFF.Tests.Controllers;

public class PagamentosControllerTests
{
    private static PagamentosController CriarController(IPagamentoService pagamentoService, INotificador notificador)
    {
        var controller = new PagamentosController(pagamentoService, notificador);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        return controller;
    }
    [Theory, AutoDomainData]
    public async Task ObterTodosAsync_ServicoRetornaLista_DeveRetornarOk(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        List<PagamentoViewModel> pagamentos)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        pagamentoService.Setup(s => s.ObterTodosAsync()).ReturnsAsync(pagamentos);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.ObterTodosAsync();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagamentos, ok.Value);
        pagamentoService.Verify(s => s.ObterTodosAsync(), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task ObterPorIdAsync_ServicoRetornaPagamento_DeveRetornarOk(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        PagamentoViewModel pagamento)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        pagamentoService.Setup(s => s.ObterPorIdAsync(id)).ReturnsAsync(pagamento);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.ObterPorIdAsync(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagamento, ok.Value);
        pagamentoService.Verify(s => s.ObterPorIdAsync(id), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task CriarPagamentoAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        PagamentoViewModel pagamento,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        pagamentoService.Setup(s => s.CriarPagamentoAsync(pagamento)).ReturnsAsync(resultado);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.CriarPagamentoAsync(pagamento);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        pagamentoService.Verify(s => s.CriarPagamentoAsync(pagamento), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task CriarPagamentoAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        PagamentoViewModel pagamento)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        pagamentoService.Setup(s => s.CriarPagamentoAsync(pagamento)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.CriarPagamentoAsync(pagamento);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task AprovarPagamentoAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        pagamentoService.Setup(s => s.AprovarPagamentoAsync(id)).ReturnsAsync(resultado);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.AprovarPagamentoAsync(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        pagamentoService.Verify(s => s.AprovarPagamentoAsync(id), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task AprovarPagamentoAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        pagamentoService.Setup(s => s.AprovarPagamentoAsync(id)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.AprovarPagamentoAsync(id);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task CancelarPagamentoAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        pagamentoService.Setup(s => s.CancelarPagamentoAsync(id)).ReturnsAsync(resultado);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.CancelarPagamentoAsync(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        pagamentoService.Verify(s => s.CancelarPagamentoAsync(id), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task CancelarPagamentoAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IPagamentoService> pagamentoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        pagamentoService.Setup(s => s.CancelarPagamentoAsync(id)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(pagamentoService.Object, notificador.Object);

        // act
        var result = await controller.CancelarPagamentoAsync(id);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
