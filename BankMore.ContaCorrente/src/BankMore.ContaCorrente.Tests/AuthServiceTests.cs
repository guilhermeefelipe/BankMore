using BankMore.ContaCorrente.Models;
using BankMore.ContaCorrente.Models.Repositories.Auth;
using BankMore.ContaCorrente.Services;
using BankMore.ContaCorrente.Services.Services.Auth;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace BankMore.ContaCorrente.Tests;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();

        // Configuração dos JWT settings de teste
        var jwtSettings = new JwtSettings
        {
            Key = "uma_chave_super_secreta_para_teste_123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };
        _jwtOptions = Options.Create(jwtSettings);

        _authService = new AuthService(_authRepositoryMock.Object, _jwtOptions);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_WhenSenhaIsIncorrect()
    {
        // Arrange
        var conta = new Models.Dto.ContaCorrente.ContaCorrenteDto
        {
            IdContaCorrente = "abc123",
            Numero = 12345,
            CPF = "11122233344",
            Salt = "salt123",
            Senha = "hashcorreto" // senha correta é diferente da que será testada
        };

        _authRepositoryMock.Setup(x => x.GetByNumeroOuCpfAsync(It.IsAny<string>()))
                           .ReturnsAsync(conta);

        // Act
        var result = await _authService.AuthenticateAsync("12345", "senhaerrada");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnJwtToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var senha = "minhasenha";
        var salt = "salt123";
        var senhaHash = CryptoHelper.Encrypt(senha, salt); // hash correto

        var conta = new Models.Dto.ContaCorrente.ContaCorrenteDto
        {
            IdContaCorrente = "abc123",
            Numero = 12345,
            CPF = "11122233344",
            Salt = salt,
            Senha = senhaHash
        };

        _authRepositoryMock.Setup(x => x.GetByNumeroOuCpfAsync(It.IsAny<string>()))
                           .ReturnsAsync(conta);

        // Act
        var token = await _authService.AuthenticateAsync("12345", senha);

        // Assert
        Assert.NotNull(token);

        // Validar que o token contém os claims esperados
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token!);

        Assert.Contains(jwtToken.Claims, c => c.Type == "idConta" && c.Value == conta.IdContaCorrente);
        Assert.Contains(jwtToken.Claims, c => c.Type == "numero" && c.Value == conta.Numero.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == "cpf" && c.Value == conta.CPF);
    }
}