using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Models.Responses;

namespace BankMore.ContaCorrente.Models.Services.ContaCorrente;

public interface IContaCorrenteService
{
    Task<int> AddAsync(CreateContaCorrenteRequest ccRequest);
    Task InactivateAsync(string idConta, string senha);
    Task<SaldoResponse> ConsultarSaldoAsync(string idContaCorrente);
    Task<string> GetIdContaCorrente(int numeroConta);

}