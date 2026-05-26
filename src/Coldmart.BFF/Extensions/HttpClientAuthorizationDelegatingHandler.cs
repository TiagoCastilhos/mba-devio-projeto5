using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Coldmart.Core.Contexts;

namespace Coldmart.BFF.Extensions
{
    [ExcludeFromCodeCoverage]
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IUsuarioContext _usuarioContext;

        public HttpClientAuthorizationDelegatingHandler(IUsuarioContext aspNetUser)
        {
            _usuarioContext = aspNetUser;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _usuarioContext.ObterUserToken();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
