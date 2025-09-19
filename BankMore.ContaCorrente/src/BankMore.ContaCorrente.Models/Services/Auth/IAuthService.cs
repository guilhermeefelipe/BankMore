using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Models.Services.Auth;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string numeroOuCpf, string senha);
}
