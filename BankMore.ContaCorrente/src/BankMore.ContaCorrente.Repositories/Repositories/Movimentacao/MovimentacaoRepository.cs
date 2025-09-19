using BankMore.ContaCorrente.Models.Dto.Movimentacao;
using BankMore.ContaCorrente.Models.Repositories.Movimentacao;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace BankMore.ContaCorrente.Repositories.Repositories.Movimentacao;

public class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly string _connectionString;

    public MovimentacaoRepository(IOptions<Database> database)
    {
        _connectionString = database.Value.ConnectionString;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public async Task InserirMovimentoAsync(MovimentacaoDto movimentacao)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = @"INSERT INTO Movimento 
                    (idmovimento, idcontacorrente, valor, tipomovimento, datamovimento) 
                  VALUES 
                    (@IdMovimento, @IdContaCorrente, @Valor, @TipoMovimento, @DataMovimento);";

        await conn.ExecuteAsync(query, movimentacao);
    }
}