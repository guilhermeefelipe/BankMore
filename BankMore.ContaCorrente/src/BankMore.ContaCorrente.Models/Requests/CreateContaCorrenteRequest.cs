using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Models.Requests;

public class CreateContaCorrenteRequest
{
    public string CPF { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Senha { get; set; } = string.Empty;
}