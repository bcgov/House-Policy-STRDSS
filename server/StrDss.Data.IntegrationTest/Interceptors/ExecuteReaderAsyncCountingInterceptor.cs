using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StrDss.Data.IntegrationTest.Interceptors;

/// <summary>
/// Counts successfully completed <see cref="DbCommand.ExecuteReaderAsync(CancellationToken)"/>
/// calls observed by Entity Framework Core.
/// </summary>
internal sealed class ExecuteReaderAsyncCountingInterceptor : DbCommandInterceptor
{
    /// <summary>
    /// Gets the number of asynchronous reader executions that completed successfully.
    /// </summary>
    public int ReadersExecuted { get; private set; }

    /// <summary>
    /// Increments the execution count and returns the reader unchanged.
    /// </summary>
    /// <param name="command">The command that was executed.</param>
    /// <param name="eventData">The execution event data.</param>
    /// <param name="result">The reader returned by the command.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The original reader.</returns>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        ReadersExecuted++;
        return ValueTask.FromResult(result);
    }
}
