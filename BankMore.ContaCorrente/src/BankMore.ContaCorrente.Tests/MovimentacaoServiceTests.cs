using BankMore.ContaCorrente.Models.Dto.ContaCorrente;
using BankMore.ContaCorrente.Models.Dto.Movimentacao;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.Movimentacao;
using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Services;
using BankMore.ContaCorrente.Services.Services.Movimentacao;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Tests;

public class MovimentacaoServiceTests
{
    private readonly Mock<IMovimentacaoRepository> _movRepoMock;
    private readonly Mock<IContaCorrenteRepository> _ccRepoMock;
    private readonly MovimentacaoService _service;

    public MovimentacaoServiceTests()
    {
        _movRepoMock = new Mock<IMovimentacaoRepository>();
        _ccRepoMock = new Mock<IContaCorrenteRepository>();
        _service = new MovimentacaoService(_movRepoMock.Object, _ccRepoMock.Object);
    }

    [Fact]
    public async Task MovimentarAsync_ShouldThrow_WhenContaNotFound()
    {
        _ccRepoMock.Setup(r => r.GetContaByNumeroAsync(It.IsAny<int>()))
                   .ReturnsAsync((ContaCorrenteDto?)null);

        var request = new MovimentacaoRequest { NumeroConta = 123, Tipo = "C", Valor = 100 };

        await Assert.ThrowsAsync<WorkException>(() => _service.MovimentarAsync(request, 123));
    }

    [Fact]
    public async Task MovimentarAsync_ShouldThrow_WhenContaInativa()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Ativo = false };
        _ccRepoMock.Setup(r => r.GetContaByNumeroAsync(123)).ReturnsAsync(conta);

        var request = new MovimentacaoRequest { NumeroConta = 123, Tipo = "C", Valor = 100 };

        await Assert.ThrowsAsync<WorkException>(() => _service.MovimentarAsync(request, 123));
    }

    [Fact]
    public async Task MovimentarAsync_ShouldThrow_WhenValorInvalido()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Ativo = true };
        _ccRepoMock.Setup(r => r.GetContaByNumeroAsync(123)).ReturnsAsync(conta);

        var request = new MovimentacaoRequest { NumeroConta = 123, Tipo = "C", Valor = 0 };

        await Assert.ThrowsAsync<WorkException>(() => _service.MovimentarAsync(request, 123));
    }

    [Theory]
    [InlineData("X")]
    [InlineData("c ")]
    [InlineData("d ")]
    public async Task MovimentarAsync_ShouldThrow_WhenTipoInvalido(string tipo)
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Ativo = true };
        _ccRepoMock.Setup(r => r.GetContaByNumeroAsync(123)).ReturnsAsync(conta);

        var request = new MovimentacaoRequest { NumeroConta = 123, Tipo = tipo, Valor = 100 };

        await Assert.ThrowsAsync<WorkException>(() => _service.MovimentarAsync(request, 123));
    }

    [Fact]
    public async Task MovimentarAsync_ShouldCallRepository_WhenValidMovimentacao()
    {
        var conta = new ContaCorrenteDto { IdContaCorrente = "abc", Ativo = true, Numero = 123 };
        _ccRepoMock.Setup(r => r.GetContaByNumeroAsync(123)).ReturnsAsync(conta);

        var request = new MovimentacaoRequest { Tipo = "C", Valor = 200 };

        await _service.MovimentarAsync(request, 123);

        _movRepoMock.Verify(r => r.InserirMovimentoAsync(It.Is<MovimentacaoDto>(
            m => m.IdContaCorrente == "abc" && m.Valor == 200 && m.TipoMovimento == "C"
        )), Times.Once);
    }
}