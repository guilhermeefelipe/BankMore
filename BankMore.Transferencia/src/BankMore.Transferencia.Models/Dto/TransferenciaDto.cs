using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transferencia.Models.Dto;

public class TransferenciaDto
{
    public string IdTransferencia { get; set; }
    public string IdContaOrigem { get; set; }
    public string IdContaDestino { get; set; }
    public string Data { get; set; }
    public decimal Valor { get; set; }

}

