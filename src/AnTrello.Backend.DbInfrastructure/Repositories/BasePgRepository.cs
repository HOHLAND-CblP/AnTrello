using System.Transactions;
using AnTrello.Backend.Domain.Entities;
using Npgsql;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public abstract class BasePgRepository 
{
    private readonly string _connectionString;
    
    public BasePgRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.MapComposite<PomodoroRound>("pomodoro_round_v1");
        await using var dataSource = dataSourceBuilder.Build();
        
        //var connection = new NpgsqlConnection(_connectionString);
        //await connection.OpenAsync();
        return await dataSource.OpenConnectionAsync();
    }
}