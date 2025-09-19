using BankMore.ContaCorrente.Models.Dto.ContaCorrente;
using BankMore.ContaCorrente.Models.Repositories.Auth;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace BankMore.ContaCorrente.Repositories.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly string _connectionString;

    public AuthRepository(IOptions<Database> database)
    {
        _connectionString = database.Value.ConnectionString;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public async Task<ContaCorrenteDto?> GetByNumeroOuCpfAsync(string numeroOuCpf)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = @"SELECT IdContaCorrente, Numero, CPF, Senha, Salt
                      FROM ContaCorrente
                      WHERE CPF = @Cpf
                         OR Numero = @Numero;";

        int? numero = int.TryParse(numeroOuCpf, out var n) ? n : null;

        return await conn.QueryFirstOrDefaultAsync<ContaCorrenteDto>(
            query,
            new { Cpf = numeroOuCpf, Numero = numero }
        );
    }
}
