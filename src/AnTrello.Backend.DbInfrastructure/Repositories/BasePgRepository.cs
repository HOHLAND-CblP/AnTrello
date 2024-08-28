using System.Transactions;
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
        
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        return connection;
    }
}