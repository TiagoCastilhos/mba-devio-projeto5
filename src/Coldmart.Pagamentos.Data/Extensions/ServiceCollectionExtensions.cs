using System.Diagnostics.CodeAnalysis;
using Coldmart.Core.Data.Extensions;
using Coldmart.Core.Data.Seeders;
using Coldmart.Pagamentos.Data.Contexts;
using Coldmart.Pagamentos.Data.Seeders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Coldmart.Pagamentos.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPagamentosData(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<IPagamentosDbContext, PagamentosDbContext>(options =>
        {
            options.ConfigureDbContextOptions(configuration, environment);
        });

        services.AddScoped<IDbSeeder, DbSeeder>();
        services.AddScoped<IDbContextSeeder, PagamentosDbContextSeeder>();

        return services;
    }
}
