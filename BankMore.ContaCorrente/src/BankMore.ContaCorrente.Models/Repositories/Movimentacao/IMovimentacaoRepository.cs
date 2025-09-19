using BankMore.ContaCorrente.Models.Dto.Movimentacao;
using BankMore.ContaCorrente.Models.Requests;

namespace BankMore.ContaCorrente.Models.Repositories.Movimentacao;

public interface IMovimentacaoRepository
{
    Task InserirMovimentoAsync(MovimentacaoDto movimentacao);
}
