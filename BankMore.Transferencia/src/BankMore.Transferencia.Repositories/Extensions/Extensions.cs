using BankMore.Transferencia.Models.Repositories.Transferencia;
using BankMore.Transferencia.Repositories.Repositories.Transferencia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.Transferencia.Repositories.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();

            return services;
        }
    }

}
