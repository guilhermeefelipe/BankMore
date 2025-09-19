using BankMore.Transferencia.Models.Dto;
using BankMore.Transferencia.Models.Repositories.Transferencia;
using BankMore.Transferencia.Models.Requests;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace BankMore.Transferencia.Repositories.Repositories.Transferencia;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly string _connectionString;

    public TransferenciaRepository(IOptions<Database> database)
    {
        _connectionString = database.Value.ConnectionString;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public async Task InserirTransferenciaAsync(TransferenciaDto transferenciaDto)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();

        var query = @"INSERT INTO Transferencia 
                    (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, valor, datamovimento) 
                  VALUES 
                    (@IdTransferencia, @IdContaOrigem, @IdContaDestino, @Valor, @Data);";

        await conn.ExecuteAsync(query, new
        {
            transferenciaDto.IdTransferencia,
            transferenciaDto.IdContaOrigem,
            transferenciaDto.IdContaDestino,
            transferenciaDto.Valor,
            transferenciaDto.Data
        });
    }
}