using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Models.Requests;

public class MovimentacaoRequest
{
    public int? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
}