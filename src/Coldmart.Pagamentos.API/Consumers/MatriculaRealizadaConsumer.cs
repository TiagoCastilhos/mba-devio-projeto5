using Coldmart.Core.Eventos;
using Coldmart.Core.Eventos;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Coldmart.Pagamentos.API.Consumers;

public class MatriculaRealizadaConsumer : IConsumer<MatriculaRealizadaEvento>
{
    private readonly IPagamentosDbContext _dbContext;

    public MatriculaRealizadaConsumer(IPagamentosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<MatriculaRealizadaEvento> context)
    {
        var jaExiste = await _dbContext.Matriculas
            .AnyAsync(m => m.Id == context.Message.MatriculaId, context.CancellationToken);

        if (jaExiste)
            return;

        var matricula = new Matricula(context.Message.MatriculaId, context.Message.AlunoId, context.Message.CursoId);
        await _dbContext.Matriculas.AddAsync(matricula, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
