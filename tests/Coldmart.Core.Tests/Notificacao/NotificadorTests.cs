using System.Diagnostics.CodeAnalysis;
using Coldmart.Core.Notificacao;
using Coldmart.Core.Tests.Attributes;

namespace Coldmart.Core.Tests.Notificacao;

[ExcludeFromCodeCoverage]
public class NotificadorTests
{
    [Theory, AutoDomainData]
    public void AdicionarErro_MensagemDeErroFornecida_DeveAdicionarMensagemDeErro(Notificador notificador, string erro)
    {
        //act
        notificador.AdicionarErro(erro);

        //assert
        var erros = notificador.ObterErros();
        Assert.Contains(erro, erros);
        Assert.True(notificador.TemErro());
    }
}
