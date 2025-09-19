using BankMore.ContaCorrente.Models;
using BankMore.ContaCorrente.Models.Services.Auth;
using BankMore.ContaCorrente.Models.Services.ContaCorrente;
using BankMore.ContaCorrente.Models.Services.Movimentacao;
using BankMore.ContaCorrente.Repositories;
using BankMore.ContaCorrente.Repositories.Extensions;
using BankMore.ContaCorrente.Services.Services.Auth;
using BankMore.ContaCorrente.Services.Services.ContaCorrente;
using BankMore.ContaCorrente.Services.Services.Movimentacao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.ContaCorrente.Services.Extensions;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories(configuration);

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<Database>(configuration.GetSection("Database"));

        services.AddScoped<IContaCorrenteService, ContaCorrenteService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMovimentacaoService, MovimentacaoService>();


        return services;
    }
}