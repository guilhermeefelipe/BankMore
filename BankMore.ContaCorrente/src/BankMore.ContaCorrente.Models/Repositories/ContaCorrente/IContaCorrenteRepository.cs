using BankMore.ContaCorrente.Models.Dto.ContaCorrente;

namespace BankMore.ContaCorrente.Models.Repositories.ContaCorrente;

public interface IContaCorrenteRepository
{
    Task<int> AddAsync(CreateContaCorrenteDto cc);
    Task<bool> ExistsByCpfAsync(string cpf);
    Task<ContaCorrenteDto?> GetByIdAsync(string idConta);
    Task<ContaCorrenteDto?> GetContaByNumeroAsync(int numeroConta);
    Task InactivateAsync(string idConta);
    Task<decimal> GetSaldoAsync(string idContaCorrente);
}
