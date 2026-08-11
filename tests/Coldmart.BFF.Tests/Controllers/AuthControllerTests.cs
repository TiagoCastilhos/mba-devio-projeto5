using Coldmart.BFF.Controllers;
using Coldmart.BFF.Services;
using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Notificacao;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Coldmart.BFF.Tests.Controllers;

public class AuthControllerTests
{
    private static AuthController CriarController(IAuthService authService, INotificador notificador)
    {
        var controller = new AuthController(authService, notificador);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    private static LogarViewModel CriarLogarViewModel() =>
        new() { Email = "usuario@teste.com", Senha = "Senha@123" };

    private static CadastrarViewModel CriarCadastrarViewModel() =>
        new() { Email = "usuario@teste.com", Senha = "Senha@123", ConfirmarSenha = "Senha@123" };

    [Fact]
    public async Task Logar_ServicoRetornaToken_DeveRetornarOk()
    {
        // arrange
        var authService = new Mock<IAuthService>();
        var notificador = new Mock<INotificador>();
        var tokenResponse = new AccessTokenResponseViewModel { AccessToken = "token-teste" };
        var viewModel = CriarLogarViewModel();

        notificador.Setup(n => n.TemErro()).Returns(false);
        authService.Setup(s => s.Logar(viewModel)).ReturnsAsync(tokenResponse);
        var controller = CriarController(authService.Object, notificador.Object);

        // act
        var result = await controller.Logar(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tokenResponse, ok.Value);
        authService.Verify(s => s.Logar(viewModel), Times.Once);
    }

    [Fact]
    public async Task Logar_ServicoRetornaNull_DeveRetornarOkComNull()
    {
        // arrange
        var authService = new Mock<IAuthService>();
        var notificador = new Mock<INotificador>();
        var viewModel = CriarLogarViewModel();

        notificador.Setup(n => n.TemErro()).Returns(false);
        authService.Setup(s => s.Logar(viewModel)).ReturnsAsync((AccessTokenResponseViewModel?)null);
        var controller = CriarController(authService.Object, notificador.Object);

        // act
        var result = await controller.Logar(viewModel);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Logar_NotificadorTemErro_DeveRetornarBadRequest()
    {
        // arrange
        var authService = new Mock<IAuthService>();
        var notificador = new Mock<INotificador>();
        var viewModel = CriarLogarViewModel();

        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        authService.Setup(s => s.Logar(viewModel)).ReturnsAsync((AccessTokenResponseViewModel?)null);
        var controller = CriarController(authService.Object, notificador.Object);

        // act
        var result = await controller.Logar(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Cadastro_ServicoRetornaToken_DeveRetornarOk()
    {
        // arrange
        var authService = new Mock<IAuthService>();
        var notificador = new Mock<INotificador>();
        var tokenResponse = new AccessTokenResponseViewModel { AccessToken = "token-teste" };
        var viewModel = CriarCadastrarViewModel();

        notificador.Setup(n => n.TemErro()).Returns(false);
        authService.Setup(s => s.Cadastro(viewModel)).ReturnsAsync(tokenResponse);
        var controller = CriarController(authService.Object, notificador.Object);

        // act
        var result = await controller.Cadastro(viewModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tokenResponse, ok.Value);
        authService.Verify(s => s.Cadastro(viewModel), Times.Once);
    }

    [Fact]
    public async Task Cadastro_NotificadorTemErro_DeveRetornarBadRequest()
    {
        // arrange
        var authService = new Mock<IAuthService>();
        var notificador = new Mock<INotificador>();
        var viewModel = CriarCadastrarViewModel();

        notificador.Setup(n => n.TemErro()).Returns(true);
        notificador.Setup(n => n.ObterErros()).Returns([]);
        authService.Setup(s => s.Cadastro(viewModel)).ReturnsAsync((AccessTokenResponseViewModel?)null);
        var controller = CriarController(authService.Object, notificador.Object);

        // act
        var result = await controller.Cadastro(viewModel);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
