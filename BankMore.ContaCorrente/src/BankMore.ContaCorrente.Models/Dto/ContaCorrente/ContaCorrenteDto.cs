namespace BankMore.ContaCorrente.Models.Dto.ContaCorrente;

public class ContaCorrenteDto
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string CPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public string Senha { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
}
