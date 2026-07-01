using Microsoft.EntityFrameworkCore;

namespace AxiCore.Persistence;

/// <summary>
/// Runs multi-step EF Core database writes inside a transaction.
/// Returns the operation result after committing so database workflows get proper lock/unlock semantics through PostgreSQL transactions.
/// </summary>
public sealed class DbTransactionRunner
{
    public async Task<T> RunAsync<T>(DbContext dbContext, Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Entering -> DbTransactionRunner.cs -> RunAsync");
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> DbTransactionRunner.cs -> RunAsync -> {ex.Message}");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> DbTransactionRunner.cs -> RunAsync");
        }
    }
}
