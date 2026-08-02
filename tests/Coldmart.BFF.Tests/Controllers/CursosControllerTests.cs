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

public class CursosControllerTests
{
    private static CursosController CriarController(ICursoService cursoService, INotificador notificador)
    {
        var controller = new CursosController(cursoService, notificador);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        return controller;
    }
    [Theory, AutoDomainData]
    public async Task ObterTodos_ServicoRetornaLista_DeveRetornarOk(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        List<CursoViewModel> cursos)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        cursoService.Setup(s => s.ObterTodos()).ReturnsAsync(cursos);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.ObterTodos();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cursos, ok.Value);
        cursoService.Verify(s => s.ObterTodos(), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task ObterPorId_ServicoRetornaCurso_DeveRetornarOk(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        CursoViewModel curso)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        cursoService.Setup(s => s.ObterPorId(id)).ReturnsAsync(curso);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.CriarCursoAsync(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(curso, ok.Value);
        cursoService.Verify(s => s.ObterPorId(id), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task CriarCursoAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        CursoViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        cursoService.Setup(s => s.CriarCursoAsync(viewModel)).ReturnsAsync(resultado);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.CriarCursoAsync(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        cursoService.Verify(s => s.CriarCursoAsync(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task CriarCursoAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        CursoViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        cursoService.Setup(s => s.CriarCursoAsync(viewModel)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.CriarCursoAsync(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task EditarCursoAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        CursoViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        cursoService.Setup(s => s.EditarCursoAsync(It.Is<CursoViewModel>(v => v.Id == id))).ReturnsAsync(resultado);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.EditarCursoAsync(id, viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        Assert.Equal(id, viewModel.Id);
        cursoService.Verify(s => s.EditarCursoAsync(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task EditarCursoAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        CursoViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        cursoService.Setup(s => s.EditarCursoAsync(It.IsAny<CursoViewModel>())).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.EditarCursoAsync(id, viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task AdicionarAulaAsync_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        AulaViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        cursoService.Setup(s => s.AdicionarAulaAsync(viewModel)).ReturnsAsync(resultado);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.AdicionarAulaAsync(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        cursoService.Verify(s => s.AdicionarAulaAsync(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task AdicionarAulaAsync_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<ICursoService> cursoService,
        [Frozen] Mock<INotificador> notificador,
        AulaViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        cursoService.Setup(s => s.AdicionarAulaAsync(viewModel)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(cursoService.Object, notificador.Object);

        // act
        var result = await controller.AdicionarAulaAsync(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
