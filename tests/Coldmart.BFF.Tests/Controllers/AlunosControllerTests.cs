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

public class AlunosControllerTests
{
    private static AlunosController CriarController(IAlunoService alunoService, INotificador notificador)
    {
        var controller = new AlunosController(alunoService, notificador);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        return controller;
    }
    [Theory, AutoDomainData]
    public async Task MatricularAoCurso_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        MatriculaViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        alunoService.Setup(s => s.MatricularAoCurso(viewModel)).ReturnsAsync(resultado);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.MatricularAoCurso(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        alunoService.Verify(s => s.MatricularAoCurso(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task MatricularAoCurso_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        MatriculaViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        alunoService.Setup(s => s.MatricularAoCurso(viewModel)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.MatricularAoCurso(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task RealizarAula_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        RealizarAulaViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        alunoService.Setup(s => s.RealizarAula(viewModel)).ReturnsAsync(resultado);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.RealizarAula(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        alunoService.Verify(s => s.RealizarAula(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task RealizarAula_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        RealizarAulaViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        alunoService.Setup(s => s.RealizarAula(viewModel)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.RealizarAula(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task Historico_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        alunoService.Setup(s => s.Historico()).ReturnsAsync(resultado);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.Historico();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        alunoService.Verify(s => s.Historico(), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task Finalizar_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        FinalizarViewModel viewModel,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        alunoService.Setup(s => s.Finalizar(viewModel)).ReturnsAsync(resultado);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.Finalizar(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        alunoService.Verify(s => s.Finalizar(viewModel), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task Finalizar_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        FinalizarViewModel viewModel)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        alunoService.Setup(s => s.Finalizar(viewModel)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.Finalizar(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory, AutoDomainData]
    public async Task Certificado_ServicoRetornaResultado_DeveRetornarOk(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id,
        ResponseResult resultado)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(false);
        alunoService.Setup(s => s.Certificado(id)).ReturnsAsync(resultado);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.Certificado(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultado, ok.Value);
        alunoService.Verify(s => s.Certificado(id), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task Certificado_NotificadorTemErro_DeveRetornarBadRequest(
        [Frozen] Mock<IAlunoService> alunoService,
        [Frozen] Mock<INotificador> notificador,
        Guid id)
    {
        // arrange
        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        alunoService.Setup(s => s.Certificado(id)).ReturnsAsync((ResponseResult?)null);
        var controller = CriarController(alunoService.Object, notificador.Object);

        // act
        var result = await controller.Certificado(id);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
