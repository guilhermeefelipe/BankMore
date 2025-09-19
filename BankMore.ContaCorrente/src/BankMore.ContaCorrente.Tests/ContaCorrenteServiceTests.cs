using BankMore.ContaCorrente.Models.Dto.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Services;
using BankMore.ContaCorrente.Services.Services.ContaCorrente;
using Moq;

namespace BankMore.ContaCorrente.Tests;

public class ContaCorrenteServiceTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly ContaCorrenteService _service;

    public ContaCorrenteServiceTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _service = new ContaCorrenteService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetIdContaCorrente_ShouldThrow_WhenContaNotFound()
    {
        _repositoryMock.Setup(r => r.GetContaByNumeroAsync(It.IsAny<int>()))
                       .ReturnsAsync((ContaCorrenteDto?)null);

        await Assert.ThrowsAsync<WorkException>(() => _service.GetIdContaCorrente(123));
    }

    [Fact]
    public async Task GetIdContaCorrente_ShouldReturnId_WhenContaExists()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc123" };
        _repositoryMock.Setup(r => r.GetContaByNumeroAsync(123))
                       .ReturnsAsync(conta);

        var result = await _service.GetIdContaCorrente(123);

        Assert.Equal("abc123", result);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenCpfInvalid()
    {
        var request = new CreateContaCorrenteRequest { CPF = "123" };
        await Assert.ThrowsAsync<WorkException>(() => _service.AddAsync(request));
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenCpfAlreadyExists()
    {
        var request = new CreateContaCorrenteRequest { CPF = "11122233344", Nome = "Teste", Senha = "123" };
        _repositoryMock.Setup(r => r.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<WorkException>(() => _service.AddAsync(request));
    }

    [Fact]
    public async Task AddAsync_ShouldReturnId_WhenCpfValid()
    {
        var request = new CreateContaCorrenteRequest { CPF = "11144477735", Nome = "Teste", Senha = "senha" }; 
        _repositoryMock.Setup(r => r.ExistsByCpfAsync(It.IsAny<string>())).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<CreateContaCorrenteDto>())).ReturnsAsync(42);

        var result = await _service.AddAsync(request);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task InactivateAsync_ShouldThrow_WhenContaNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((ContaCorrenteDto?)null);
        await Assert.ThrowsAsync<WorkException>(() => _service.InactivateAsync("abc", "senha"));
    }

    [Fact]
    public async Task InactivateAsync_ShouldThrow_WhenSenhaInvalid()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Salt = "salt", Senha = "hashcorreto" };
        _repositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(conta);

        await Assert.ThrowsAsync<WorkException>(() => _service.InactivateAsync("abc", "senhaerrada"));
    }

    [Fact]
    public async Task InactivateAsync_ShouldCallRepository_WhenValid()
    {
        var senha = "minhasenha";
        var salt = "salt123";
        var conta = new ContaCorrenteDto
        {
            IdContaCorrente = "abc",
            Salt = salt,
            Senha = CryptoHelper.Encrypt(senha, salt)
        };
        _repositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(conta);

        await _service.InactivateAsync("abc", senha);

        _repositoryMock.Verify(r => r.InactivateAsync("abc"), Times.Once);
    }

    [Fact]
    public async Task ConsultarSaldoAsync_ShouldThrow_WhenContaNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((ContaCorrenteDto?)null);
        await Assert.ThrowsAsync<WorkException>(() => _service.ConsultarSaldoAsync("abc"));
    }

    [Fact]
    public async Task ConsultarSaldoAsync_ShouldThrow_WhenContaInactive()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Ativo = false };
        _repositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(conta);
        await Assert.ThrowsAsync<WorkException>(() => _service.ConsultarSaldoAsync("abc"));
    }

    [Fact]
    public async Task ConsultarSaldoAsync_ShouldReturnSaldo_WhenContaActive()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Numero = 123, Nome = "Teste", Ativo = true };
        _repositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(conta);
        _repositoryMock.Setup(r => r.GetSaldoAsync("abc")).ReturnsAsync(1000m);

        var result = await _service.ConsultarSaldoAsync("abc");

        Assert.Equal(123, result.NumeroConta);
        Assert.Equal("Teste", result.Nome);
        Assert.Equal(1000m, result.Saldo);
    }
}