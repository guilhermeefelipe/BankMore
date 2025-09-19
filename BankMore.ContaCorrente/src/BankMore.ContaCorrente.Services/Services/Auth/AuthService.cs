using BankMore.ContaCorrente.Models;
using BankMore.ContaCorrente.Models.Repositories.Auth;
using BankMore.ContaCorrente.Models.Services.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.ContaCorrente.Services.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtSettings jwt;
    public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwt)
    {
        _authRepository = authRepository;
        this.jwt = jwt.Value;
    }

    public async Task<string?> AuthenticateAsync(string numeroOuCpf, string senha)
    {
        var conta = await _authRepository.GetByNumeroOuCpfAsync(numeroOuCpf);

        if (conta is null)
            return null;

        var senhaHash = CryptoHelper.Encrypt(senha, conta.Salt);
        if (senhaHash != conta.Senha)
            return null;

        return GenerateJwtToken(conta.IdContaCorrente, conta.Numero, conta.CPF);
    }

    private string GenerateJwtToken(string idConta, int numero, string cpf)
    {
        var claims = new[]
        {
            new Claim("idConta", idConta),
            new Claim("numero", numero.ToString()),
            new Claim("cpf", cpf)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}