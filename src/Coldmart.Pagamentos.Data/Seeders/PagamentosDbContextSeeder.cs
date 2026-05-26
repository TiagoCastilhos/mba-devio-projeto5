using System.Diagnostics.CodeAnalysis;
using Coldmart.Core.Data.Seeders;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Pagamentos.Data.Seeders;

[ExcludeFromCodeCoverage]
internal class PagamentosDbContextSeeder : IDbContextSeeder
{
    private readonly IPagamentosDbContext _pagamentosDbContext;

    public PagamentosDbContextSeeder(IPagamentosDbContext pagamentosDbContext)
    {
        _pagamentosDbContext = pagamentosDbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _pagamentosDbContext.Matriculas.AnyAsync(cancellationToken))
            return;

        var matricula = new Matricula(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Guid.Parse("10890b08-2e51-43d0-84d2-244041b6c10c"),
            Guid.Parse("11111111-1111-1111-1111-111111111111"));

        await _pagamentosDbContext.Matriculas.AddAsync(matricula, cancellationToken);
        await _pagamentosDbContext.SaveChangesAsync(cancellationToken);
    }
}
