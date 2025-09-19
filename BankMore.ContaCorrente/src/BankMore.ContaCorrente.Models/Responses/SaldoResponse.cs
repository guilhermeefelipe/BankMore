namespace BankMore.ContaCorrente.Models.Responses;

public class SaldoResponse
{
    public int NumeroConta { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string DataConsulta { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}