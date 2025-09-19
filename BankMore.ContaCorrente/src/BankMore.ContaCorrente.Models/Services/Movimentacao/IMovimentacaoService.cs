using BankMore.ContaCorrente.Models.Requests;

namespace BankMore.ContaCorrente.Models.Services.Movimentacao
{
    public interface IMovimentacaoService
    {
        Task MovimentarAsync(MovimentacaoRequest request, int numeroContaLogado);
    }
}
