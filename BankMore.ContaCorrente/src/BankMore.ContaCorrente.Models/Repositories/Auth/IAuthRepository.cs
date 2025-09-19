using BankMore.ContaCorrente.Models.Dto.ContaCorrente;

namespace BankMore.ContaCorrente.Models.Repositories.Auth;

public interface IAuthRepository
{
    Task<ContaCorrenteDto?> GetByNumeroOuCpfAsync(string numeroOuCpf);
}