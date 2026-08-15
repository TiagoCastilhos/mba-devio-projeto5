using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Xunit2;
using Coldmart.Auth.Business.Services;
using Coldmart.Auth.Business.ViewModels;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Options;
using Coldmart.Core.Tests.Attributes;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Coldmart.Auth.Business.Tests.Services;

[ExcludeFromCodeCoverage]
public class AutenticacaoServiceTests
{
    [Theory, AutoDomainData]
    public async Task GerarTokenAsync_CredenciaisValidas_DeveRetornarToken(
        [Frozen] Mock<UserManager<IdentityUser>> userManager,
        [Frozen] Mock<SignInManager<IdentityUser>> signInManager,
        [Frozen] Mock<INotificador> notificador,
        IdentityUser user,
        LogarViewModel viewModel,
        IFixture fixture)
    {
        //arrange
        userManager.Setup(um => um.FindByEmailAsync(viewModel.Email))
            .ReturnsAsync(user);

        signInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, viewModel.Senha, false))
            .ReturnsAsync(SignInResult.Success);

        var autenticacaoService = new AutenticacaoService(userManager.Object, signInManager.Object, fixture.Create<JwtOptions>(), notificador.Object);

        //act
        var token = await autenticacaoService.GerarTokenAsync(viewModel);

        //assert
        Assert.NotNull(token);
        notificador.Verify(n => n.AdicionarErro(It.IsAny<string>()), Times.Never);
    }

    [Theory, AutoDomainData]
    public async Task GerarTokenAsync_UsuarioInexistente_DeveAdicionarErro(
        [Frozen] Mock<UserManager<IdentityUser>> userManager,
        [Frozen] Mock<SignInManager<IdentityUser>> signInManager,
        [Frozen] Mock<INotificador> notificador,
        LogarViewModel viewModel,
        IFixture fixture)
    {
        //arrange
        userManager.Setup(um => um.FindByEmailAsync(viewModel.Email))
            .ReturnsAsync(() => null);

        var autenticacaoService = new AutenticacaoService(userManager.Object, signInManager.Object, fixture.Create<JwtOptions>(), notificador.Object);

        //act
        var token = await autenticacaoService.GerarTokenAsync(viewModel);

        //assert
        Assert.Null(token);
        notificador.Verify(n => n.AdicionarErro("Não foi possível autenticar o usuário. Verifique as credenciais e tente novamente."), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task GerarTokenAsync_SenhaIncorreta_DeveAdicionarErro(
        [Frozen] Mock<UserManager<IdentityUser>> userManager,
        [Frozen] Mock<SignInManager<IdentityUser>> signInManager,
        [Frozen] Mock<INotificador> notificador,
        LogarViewModel viewModel,
        IFixture fixture)
    {
        //arrange
        signInManager.Setup(sm => sm.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), viewModel.Senha, false))
            .ReturnsAsync(SignInResult.Failed);

        var autenticacaoService = new AutenticacaoService(userManager.Object, signInManager.Object, fixture.Create<JwtOptions>(), notificador.Object);

        //act
        var token = await autenticacaoService.GerarTokenAsync(viewModel);

        //assert
        Assert.Null(token);
        notificador.Verify(n => n.AdicionarErro("Não foi possível autenticar o usuário. Verifique as credenciais e tente novamente."), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task GerarTokenAsync_UsuarioBloqueado_DeveAdicionarErro(
        [Frozen] Mock<UserManager<IdentityUser>> userManager,
        [Frozen] Mock<SignInManager<IdentityUser>> signInManager,
        [Frozen] Mock<INotificador> notificador,
        LogarViewModel viewModel,
        IFixture fixture)
    {
        //arrange
        signInManager.Setup(sm => sm.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), viewModel.Senha, false))
            .ReturnsAsync(SignInResult.LockedOut);

        var autenticacaoService = new AutenticacaoService(userManager.Object, signInManager.Object, fixture.Create<JwtOptions>(), notificador.Object);

        //act
        var token = await autenticacaoService.GerarTokenAsync(viewModel);

        //assert
        Assert.Null(token);
        notificador.Verify(n => n.AdicionarErro("Usuário está bloqueado devido ao excesso de tentativas."), Times.Once);
    }
}
