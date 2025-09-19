namespace BankMore.ContaCorrente.Models.Dto.Movimentacao;

public class MovimentacaoDto
{
    public string IdMovimento { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public string DataMovimento { get; set; } = string.Empty;
    public string TipoMovimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
