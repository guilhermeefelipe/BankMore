using BankMore.ContaCorrente.Models.Dto.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.ContaCorrente;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace BankMore.ContaCorrente.Repositories.Repositories.ContaCorrente;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;

    public ContaCorrenteRepository(IOptions<Database> database, IMemoryCache cache)
    {
        _connectionString = database.Value.ConnectionString;
        _cache = cache;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public async Task<ContaCorrenteDto?> GetByIdAsync(string idConta)
    {
        var cacheKey = $"Conta_{idConta}";
        if (!_cache.TryGetValue(cacheKey, out ContaCorrenteDto conta))
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var query = @"SELECT IdContaCorrente, Numero, CPF, Senha, Salt, Ativo, Nome
                          FROM ContaCorrente
                          WHERE IdContaCorrente = @Id;";

            conta = await conn.QueryFirstOrDefaultAsync<ContaCorrenteDto>(query, new { Id = idConta });

            if (conta != null)
            {
                _cache.Set(cacheKey, conta, TimeSpan.FromMinutes(10));
                _cache.Set($"Conta_Numero_{conta.Numero}", conta, TimeSpan.FromMinutes(10));
            }
        }

        return conta;
    }

    public async Task<ContaCorrenteDto?> GetContaByNumeroAsync(int numeroConta)
    {
        var cacheKey = $"Conta_Numero_{numeroConta}";
        if (!_cache.TryGetValue(cacheKey, out ContaCorrenteDto conta))
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var query = @"SELECT IdContaCorrente, Numero, CPF, Senha, Salt, Ativo, Nome
                          FROM ContaCorrente
                          WHERE Numero = @Numero;";

            conta = await conn.QueryFirstOrDefaultAsync<ContaCorrenteDto>(query, new { Numero = numeroConta });

            if (conta != null)
            {
                _cache.Set(cacheKey, conta, TimeSpan.FromMinutes(10));
                _cache.Set($"Conta_{conta.IdContaCorrente}", conta, TimeSpan.FromMinutes(10));
            }
        }

        return conta;
    }

    public async Task<bool> ExistsByCpfAsync(string cpf)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = "SELECT 1 FROM ContaCorrente WHERE CPF = @Cpf LIMIT 1;";
        var result = await conn.ExecuteScalarAsync<int?>(query, new { Cpf = cpf });
        return result.HasValue;
    }

    public async Task<int> AddAsync(CreateContaCorrenteDto cc)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var nextNumero = await conn.ExecuteScalarAsync<int>(
            "SELECT IFNULL(MAX(Numero), 0) + 1 FROM ContaCorrente;"
        );

        var query = @"INSERT INTO ContaCorrente 
                 (IdContaCorrente, Numero, Nome, Ativo, CPF, Senha, Salt) 
              VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @CPF, @Senha, @Salt);";

        await conn.ExecuteAsync(query, new
        {
            Numero = nextNumero,
            IdContaCorrente = Guid.NewGuid(),
            cc.Nome,
            cc.Ativo,
            cc.CPF,
            cc.Senha,
            cc.Salt
        });

        var numero = await conn.ExecuteScalarAsync<int>(
            "SELECT Numero FROM ContaCorrente WHERE CPF = @CPF;",
            new { cc.CPF });

        return numero;
    }

    public async Task InactivateAsync(string idConta)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = @"UPDATE ContaCorrente SET Ativo = 0 WHERE IdContaCorrente = @Id;";
        await conn.ExecuteAsync(query, new { Id = idConta });

        _cache.Remove($"Conta_{idConta}");

        var conta = await GetByIdAsync(idConta);
        if (conta != null)
        {
            _cache.Remove($"Conta_Numero_{conta.Numero}");
        }
    }

    public async Task<decimal> GetSaldoAsync(string idContaCorrente)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = @"SELECT 
                        COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                        COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) AS Saldo
                      FROM movimento
                      WHERE idcontacorrente = @idContaCorrente";

        return await conn.ExecuteScalarAsync<decimal>(query, new { idContaCorrente });
    }
}
