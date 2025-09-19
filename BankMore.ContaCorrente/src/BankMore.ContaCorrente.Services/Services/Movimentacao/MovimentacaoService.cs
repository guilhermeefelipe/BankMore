using BankMore.ContaCorrente.Models.Dto.Movimentacao;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.Movimentacao;
using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Models.Responses;
using BankMore.ContaCorrente.Models.Services.Movimentacao;

namespace BankMore.ContaCorrente.Services.Services.Movimentacao;

public class MovimentacaoService : IMovimentacaoService
{
    private readonly IMovimentacaoRepository movimentacaoRepository;
    private readonly IContaCorrenteRepository contaCorrenteRepository;

    public MovimentacaoService(IMovimentacaoRepository movimentacaoRepository, IContaCorrenteRepository contaCorrenteRepository)
    {
        this.movimentacaoRepository = movimentacaoRepository;
        this.contaCorrenteRepository = contaCorrenteRepository;
    }

    public async Task MovimentarAsync(MovimentacaoRequest request, int numeroContaLogado)
    {
        int numeroConta = request.NumeroConta ?? numeroContaLogado;

        request.Tipo = request.Tipo.ToUpper();

        var conta = await contaCorrenteRepository.GetContaByNumeroAsync(numeroConta);
        if (conta == null)
            throw new WorkException("INVALID_ACCOUNT", "Conta corrente não encontrada.");

        if (conta.Ativo != true)
            throw new WorkException("INACTIVE_ACCOUNT", "Conta corrente inativa.");

        if (request.Valor <= 0)
            throw new WorkException("INVALID_VALUE", "O valor da movimentação deve ser positivo.");

        if (request.Tipo != "C" && request.Tipo != "D")
            throw new WorkException("INVALID_TYPE", "Tipo de movimentação inválido.");

        if (numeroConta != null && numeroConta != request.NumeroConta && request.Tipo != "C")
            throw new WorkException("INVALID_TYPE", "Apenas crédito é permitido em contas de terceiros.");

        var movimentacaoDto = new MovimentacaoDto
        {
            IdMovimento = Guid.NewGuid().ToString(),
            IdContaCorrente = conta.IdContaCorrente,
            DataMovimento = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
            TipoMovimento = request.Tipo,
            Valor = request.Valor
        };

        await movimentacaoRepository.InserirMovimentoAsync(movimentacaoDto);
    }
}
