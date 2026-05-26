using System.Diagnostics.CodeAnalysis;
using Coldmart.Core.Extensions;
using Coldmart.Pagamentos.API.Consumers;
using Coldmart.Pagamentos.Business.Services;
using Coldmart.Pagamentos.Data.Extensions;
using MassTransit;

namespace Coldmart.Pagamentos.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigurarInjecaoDependencia(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services
            .AddPagamentosData(configuration, environment);

        services.AddScoped<IPagamentoQueries, PagamentoQueries>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<PagamentosService>();
        });

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MatriculaRealizadaConsumer>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("pagamentos", false));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration.GetValue("RabbitMq:Username", "coldmart"));
                    h.Password(configuration.GetValue("RabbitMq:Password", "coldmart"));
                });

                cfg.UseMessageRetry(r =>
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ConfigureEndpoints(context);
            });
        });


        services
            .AddCoreServices(configuration);

        return services;
    }
}
