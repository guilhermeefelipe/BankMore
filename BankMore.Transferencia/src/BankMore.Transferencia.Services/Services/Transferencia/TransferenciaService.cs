using BankMore.Transferencia.Models;
using BankMore.Transferencia.Models.Dto;
using BankMore.Transferencia.Models.Repositories.Transferencia;
using BankMore.Transferencia.Models.Requests;
using BankMore.Transferencia.Models.Services.Transferencia;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transferencia.Services.Services.Transferencia;

public class TransferenciaService : ITransferenciaService
{
    private readonly ITransferenciaRepository transferenciaRepository;
    private readonly ServicesSettings servicesSettings;
    private readonly HttpClient httpClient;

    public TransferenciaService(ITransferenciaRepository transferenciaRepository, IOptions<ServicesSettings> servicesSettings, HttpClient httpClient)
    {
        this.transferenciaRepository = transferenciaRepository;
        this.httpClient = httpClient;
        this.servicesSettings = servicesSettings.Value;
    }

    public async Task EfetuarTransferenciaAsync(TransferenciaRequest request, int numeroContaOrigem, string idContaOrigem, string token)
    {
        if (request.Valor <= 0)
            throw new WorkException("INVALID_VALUE", "O valor da transferência deve ser positivo.");

        var debitoResponse = await ChamarContaCorrenteAsync("debito", numeroContaOrigem, request.Valor, token);
        if (!debitoResponse.IsSuccessStatusCode)
            throw new WorkException("DEBIT_FAILED", "Falha ao debitar a conta de origem.");

        var creditoResponse = await ChamarContaCorrenteAsync("credito", request.NumeroContaDestino, request.Valor, token);
        if (!creditoResponse.IsSuccessStatusCode)
        {
            await ChamarContaCorrenteAsync("credito", numeroContaOrigem, request.Valor, token);
            throw new WorkException("CREDIT_FAILED", "Falha ao creditar a conta de destino. Estorno realizado.");
        }

        var idContaDestinoResponse = await GetIdContaCorrenteAsync(request.NumeroContaDestino, token);

        if (!idContaDestinoResponse.IsSuccessStatusCode)
            throw new WorkException("DESTINATION_ACCOUNT_NOT_FOUND", "Conta de destino não encontrada.");

        idContaDestinoResponse.EnsureSuccessStatusCode();

        var idContaDestino = await idContaDestinoResponse.Content.ReadAsStringAsync();

        var transferenciaDto = new TransferenciaDto
        {
            IdTransferencia = Guid.NewGuid().ToString(),
            Data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            IdContaDestino = idContaDestino,
            IdContaOrigem = idContaOrigem,
            Valor = request.Valor
        };

        await transferenciaRepository.InserirTransferenciaAsync(transferenciaDto);
    }

    private async Task<HttpResponseMessage> ChamarContaCorrenteAsync(string tipo, int numeroConta, decimal valor, string token)
    {
        var payload = new
        {
            NumeroConta = numeroConta,
            Valor = valor,
            Tipo = tipo.StartsWith("d", StringComparison.OrdinalIgnoreCase) ? "D" : "C"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, servicesSettings.ContaCorrenteApi + "/api/movimentacao")
        {
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.SendAsync(request);
    }

    private async Task<HttpResponseMessage> GetIdContaCorrenteAsync(int numeroConta, string token)
    {
        var url = $"{servicesSettings.ContaCorrenteApi}/api/ContaCorrente/{numeroConta}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.SendAsync(request);
    }
}