using BankMore.Transferencia.Models;
using BankMore.Transferencia.Models.Dto;
using BankMore.Transferencia.Models.Repositories.Transferencia;
using BankMore.Transferencia.Models.Requests;
using BankMore.Transferencia.Services;
using BankMore.Transferencia.Services.Services.Transferencia;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace BankMore.Transferencia.Tests;

public class TransferenciaServiceTests
{
    private readonly Mock<ITransferenciaRepository> _repositoryMock;
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly IOptions<ServicesSettings> _settings;

    public TransferenciaServiceTests()
    {
        _repositoryMock = new Mock<ITransferenciaRepository>();

        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object);

        _settings = Options.Create(new ServicesSettings
        {
            ContaCorrenteApi = "http://fake-api"
        });
    }

    [Fact]
    public async Task EfetuarTransferenciaAsync_ValorNegativo_ThrowsWorkException()
    {
        // Arrange
        var service = new TransferenciaService(_repositoryMock.Object, _settings, _httpClient);
        var request = new TransferenciaRequest { Valor = -10, NumeroContaDestino = 123 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<WorkException>(() =>
            service.EfetuarTransferenciaAsync(request, 1, "origem-1", "token"));

        Assert.Equal("INVALID_VALUE", ex.ErrorType);
    }

    [Fact]
    public async Task EfetuarTransferenciaAsync_DebitoFalha_ThrowsWorkException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.BadRequest, HttpStatusCode.OK);

        var service = new TransferenciaService(_repositoryMock.Object, _settings, _httpClient);
        var request = new TransferenciaRequest { Valor = 100, NumeroContaDestino = 123 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<WorkException>(() =>
            service.EfetuarTransferenciaAsync(request, 1, "origem-1", "token"));

        Assert.Equal("DEBIT_FAILED", ex.ErrorType);
    }

    [Fact]
    public async Task EfetuarTransferenciaAsync_ContaDestinoNaoEncontrada_ThrowsWorkException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.NotFound);

        var service = new TransferenciaService(_repositoryMock.Object, _settings, _httpClient);
        var request = new TransferenciaRequest { Valor = 100, NumeroContaDestino = 123 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<WorkException>(() =>
            service.EfetuarTransferenciaAsync(request, 1, "origem-1", "token"));

        Assert.Equal("DESTINATION_ACCOUNT_NOT_FOUND", ex.ErrorType);
    }

    [Fact]
    public async Task EfetuarTransferenciaAsync_Sucesso_ChamaRepository()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK, "destino-123");

        var service = new TransferenciaService(_repositoryMock.Object, _settings, _httpClient);
        var request = new TransferenciaRequest { Valor = 100, NumeroContaDestino = 123 };

        // Act
        await service.EfetuarTransferenciaAsync(request, 1, "origem-1", "token");

        // Assert
        _repositoryMock.Verify(r => r.InserirTransferenciaAsync(It.IsAny<TransferenciaDto>()), Times.Once);
    }

    private void SetupHttpResponse(params object[] responses)
    {
        var setupSequence = _httpHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        foreach (var response in responses)
        {
            if (response is HttpStatusCode statusCode)
            {
                setupSequence = setupSequence.ReturnsAsync(new HttpResponseMessage(statusCode));
            }
            else if (response is string content)
            {
                setupSequence = setupSequence.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
