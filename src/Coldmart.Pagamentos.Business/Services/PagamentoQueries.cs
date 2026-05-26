using Coldmart.Pagamentos.Business.Extensions;
using Coldmart.Pagamentos.Business.ViewModels;
using Coldmart.Pagamentos.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Pagamentos.Business.Services;

public class PagamentoQueries : IPagamentoQueries
{
    private readonly IPagamentosDbContext _context;

    public PagamentoQueries(IPagamentosDbContext context)
    {
        _context = context;
    }

    public async Task<PagamentoViewModel?> ObterPorIdAsync(Guid id)
    {
        var pagamento = await _context.Pagamentos.FindAsync(id);
        return pagamento?.ToViewModel();
    }

    public async Task<IEnumerable<PagamentoViewModel>> ObterTodosAsync()
    {
        var pagamentos = await _context.Pagamentos.ToListAsync();
        return pagamentos.ToViewModel();
    }
}
