using BankMore.Transferencia.Models.Dto;
using BankMore.Transferencia.Models.Requests;

namespace BankMore.Transferencia.Models.Repositories.Transferencia
{
    public interface ITransferenciaRepository
    {
        Task InserirTransferenciaAsync(TransferenciaDto transferenciaDto);
    }

}
