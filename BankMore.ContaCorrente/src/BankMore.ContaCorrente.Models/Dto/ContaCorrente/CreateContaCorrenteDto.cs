namespace BankMore.ContaCorrente.Models.Dto.ContaCorrente;


public class CreateContaCorrenteDto
{
    public string CPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public string Senha { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
}