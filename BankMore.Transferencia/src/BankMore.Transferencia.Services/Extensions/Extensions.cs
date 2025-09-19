using BankMore.Transferencia.Models;
using BankMore.Transferencia.Models.Services.Transferencia;
using BankMore.Transferencia.Repositories;
using BankMore.Transferencia.Repositories.Extensions;
using BankMore.Transferencia.Services.Services.Transferencia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Transferencia.Services.Extensions;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories(configuration);

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<ServicesSettings>(configuration.GetSection("Services"));
        services.Configure<Database>(configuration.GetSection("Database"));

        services.AddScoped<ITransferenciaService, TransferenciaService>();

        return services;
    }
}
