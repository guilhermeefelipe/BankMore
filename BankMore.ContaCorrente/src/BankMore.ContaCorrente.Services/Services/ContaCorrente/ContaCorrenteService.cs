using BankMore.ContaCorrente.Models.Dto.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Models.Responses;
using BankMore.ContaCorrente.Models.Services.ContaCorrente;
using BankMore.ContaCorrente.Repositories.Repositories.Movimentacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Services.Services.ContaCorrente;

public class ContaCorrenteService : IContaCorrenteService
{
    private readonly IContaCorrenteRepository contaCorrenteRepository;

    public ContaCorrenteService(IContaCorrenteRepository contaCorrenteRepository)
    {
        this.contaCorrenteRepository = contaCorrenteRepository;
    }

    public async Task<string> GetIdContaCorrente(int numeroConta)
    {
        var conta = await contaCorrenteRepository.GetContaByNumeroAsync(numeroConta);

        if (conta == null)
            throw new WorkException("INVALID_ACCOUNT", "Conta inválida");

        return conta.IdContaCorrente;
    }

    public async Task<int> AddAsync(CreateContaCorrenteRequest ccRequest)
    {
        var formatedCpf = GetValidCpf(ccRequest.CPF);

        if (string.IsNullOrEmpty(formatedCpf))
            throw new WorkException("INVALID_DOCUMENT", "CPF invalido.");

        if (await contaCorrenteRepository.ExistsByCpfAsync(formatedCpf))
            throw new WorkException("INVALID_DOCUMENT","Este CPF ja esta cadastrado.");

        var ccDto = new CreateContaCorrenteDto
        {
            CPF = formatedCpf,
            Nome = ccRequest.Nome,
            Ativo = true
        };

        ccDto.Salt = CryptoHelper.CreateSalt();
        ccDto.Senha = CryptoHelper.Encrypt(ccRequest.Senha, ccDto.Salt);

        return await contaCorrenteRepository.AddAsync(ccDto);
    }

    public async Task InactivateAsync(string idConta, string senha)
    {
        var conta = await contaCorrenteRepository.GetByIdAsync(idConta);

        if (conta == null)
            throw new WorkException("INVALID_ACCOUNT", "Conta inválida");

        var senhaHash = CryptoHelper.Encrypt(senha, conta.Salt);
        if (senhaHash != conta.Senha)
            throw new WorkException("USER_UNAUTHORIZED", "Senha inválida");

        await contaCorrenteRepository.InactivateAsync(conta.IdContaCorrente);
    }
    public async Task<SaldoResponse> ConsultarSaldoAsync(string idContaCorrente)
    {
        var conta = await contaCorrenteRepository.GetByIdAsync(idContaCorrente);

        if (conta == null)
            throw new WorkException("INVALID_ACCOUNT", "Conta corrente não encontrada.");

        if (!conta.Ativo)
            throw new WorkException("INACTIVE_ACCOUNT", "Conta corrente inativa.");

        var saldo = await contaCorrenteRepository.GetSaldoAsync(idContaCorrente);

        return new SaldoResponse
        {
            NumeroConta = conta.Numero,
            Nome = conta.Nome,
            DataConsulta = DateTime.UtcNow.ToString(),
            Saldo = saldo
        };
    }
    private static string GetValidCpf(string cpf)
    {
        cpf = Regex.Replace(cpf, @"[^\d]", ""); 

        if (cpf.Length != 11)
            return string.Empty;

        if (new string(cpf[0], cpf.Length) == cpf)
            return string.Empty;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito) ? cpf : string.Empty;
    }

}
