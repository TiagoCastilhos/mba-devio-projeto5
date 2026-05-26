using System.Diagnostics.CodeAnalysis;
using Coldmart.BFF.Models;
using Coldmart.BFF.Services.Interfaces;
using Coldmart.BFF.ViewModels;
using Coldmart.Core.Communication;
using Microsoft.Extensions.Options;

namespace Coldmart.BFF.Services;

[ExcludeFromCodeCoverage]
public class AlunoService : Service, IAlunoService
{
    private readonly HttpClient _httpClient;

    public AlunoService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.AlunosServiceUrl);
    }

    public async Task<ResponseResult?> MatricularAoCurso(MatriculaViewModel viewModel)
    {
        var matriculaContent = ObterConteudo(viewModel);
        var response = await _httpClient.PostAsync("api/alunos/cursos", matriculaContent);
        return await DeserializarObjetoResponse<ResponseResult>(response);
    }

    public async Task<ResponseResult?> RealizarAula(RealizarAulaViewModel viewModel)
    {
        var realizarAulaContent = ObterConteudo(viewModel);
        var response = await _httpClient.PutAsync("api/alunos/cursos/aulas", realizarAulaContent);
        return await DeserializarObjetoResponse<ResponseResult>(response);
    }

    public async Task<ResponseResult?> Certificado(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/alunos/certificado/{id}");
        return await DeserializarObjetoResponse<ResponseResult>(response);
    }

    public async Task<ResponseResult?> Finalizar(FinalizarViewModel viewModel)
    {
        var finalizarContent = ObterConteudo(viewModel);
        var response = await _httpClient.PostAsync("api/alunos/finalizar", finalizarContent);
        return await DeserializarObjetoResponse<ResponseResult>(response);
    }

    public async Task<ResponseResult?> Historico()
    {
        var response = await _httpClient.GetAsync("api/alunos/historico");
        return await DeserializarObjetoResponse<ResponseResult>(response);
    }

}
