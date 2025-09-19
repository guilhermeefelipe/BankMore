using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Models.Dto.Auth;

public class LoginRequest
{
    public string NumeroOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
