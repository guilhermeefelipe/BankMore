using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transferencia.Models.Requests
{
    public class TransferenciaRequest
    {
        public int NumeroContaDestino { get; set; }                 
        public decimal Valor { get; set; }                   
    }
}
