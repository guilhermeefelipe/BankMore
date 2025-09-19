using BankMore.ContaCorrente.Models.Repositories.Auth;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.Movimentacao;
using BankMore.ContaCorrente.Models.Services.Auth;
using BankMore.ContaCorrente.Repositories.Repositories.Auth;
using BankMore.ContaCorrente.Repositories.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Repositories.Repositories.Movimentacao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.ContaCorrente.Repositories.Extensions;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

        return services;
    }
}
