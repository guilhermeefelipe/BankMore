using BankMore.Transferencia.Models.Requests;

namespace BankMore.Transferencia.Models.Services.Transferencia
{
    public interface ITransferenciaService
    {
        Task EfetuarTransferenciaAsync(TransferenciaRequest request, int numeroContaOrigem, string idContaOrigem, string token);
    }
}
