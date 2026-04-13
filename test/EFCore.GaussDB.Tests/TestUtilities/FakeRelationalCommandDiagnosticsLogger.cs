#nullable enable

using System.Data.Common;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class FakeRelationalCommandDiagnosticsLogger
    : FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>, IRelationalCommandDiagnosticsLogger
{
    public InterceptionResult<DbCommand> CommandCreating(
        IRelationalConnection connection,
        DbCommandMethod commandMethod,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => default;

    public DbCommand CommandCreated(
        IRelationalConnection connection,
        DbCommand command,
        DbCommandMethod commandMethod,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => command;

    public DbCommand CommandInitialized(
        IRelationalConnection connection,
        DbCommand command,
        DbCommandMethod commandMethod,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => command;

    public InterceptionResult<DbDataReader> CommandReaderExecuting(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => CommandReaderExecuting(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource);

    public InterceptionResult<DbDataReader> CommandReaderExecuting(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => default;

    public InterceptionResult<object> CommandScalarExecuting(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => CommandScalarExecuting(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource);

    public InterceptionResult<object> CommandScalarExecuting(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => default;

    public InterceptionResult<int> CommandNonQueryExecuting(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => CommandNonQueryExecuting(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource);

    public InterceptionResult<int> CommandNonQueryExecuting(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource)
        => default;

    public ValueTask<InterceptionResult<DbDataReader>> CommandReaderExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandReaderExecutingAsync(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource, cancellationToken);

    public ValueTask<InterceptionResult<DbDataReader>> CommandReaderExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => default;

    public ValueTask<InterceptionResult<object>> CommandScalarExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandScalarExecutingAsync(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource, cancellationToken);

    public ValueTask<InterceptionResult<object>> CommandScalarExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => default;

    public ValueTask<InterceptionResult<int>> CommandNonQueryExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandNonQueryExecutingAsync(connection, command, string.Empty, context, commandId, connectionId, startTime, commandSource, cancellationToken);

    public ValueTask<InterceptionResult<int>> CommandNonQueryExecutingAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => default;

    public DbDataReader CommandReaderExecuted(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DbDataReader methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => CommandReaderExecuted(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource);

    public DbDataReader CommandReaderExecuted(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DbDataReader methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => methodResult;

    public object? CommandScalarExecuted(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        object? methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => CommandScalarExecuted(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource);

    public object? CommandScalarExecuted(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        object? methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => methodResult;

    public int CommandNonQueryExecuted(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        int methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => CommandNonQueryExecuted(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource);

    public int CommandNonQueryExecuted(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        int methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => methodResult;

    public ValueTask<DbDataReader> CommandReaderExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DbDataReader methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandReaderExecutedAsync(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource, cancellationToken);

    public ValueTask<DbDataReader> CommandReaderExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        DbDataReader methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => new(methodResult);

    public ValueTask<object?> CommandScalarExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        object? methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandScalarExecutedAsync(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource, cancellationToken);

    public ValueTask<object?> CommandScalarExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        object? methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => new(methodResult);

    public ValueTask<int> CommandNonQueryExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        int methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandNonQueryExecutedAsync(connection, command, string.Empty, context, commandId, connectionId, methodResult, startTime, duration, commandSource, cancellationToken);

    public ValueTask<int> CommandNonQueryExecutedAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        Guid commandId,
        Guid connectionId,
        int methodResult,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => new(methodResult);

    public void CommandException(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
    {
    }

    public Task CommandExceptionAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void CommandError(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => CommandError(connection, command, string.Empty, context, executeMethod, commandId, connectionId, exception, startTime, duration, commandSource);

    public void CommandError(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
    {
    }

    public Task CommandErrorAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandErrorAsync(connection, command, string.Empty, context, executeMethod, commandId, connectionId, exception, startTime, duration, commandSource, cancellationToken);

    public Task CommandErrorAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        Exception exception,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void CommandCanceled(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
        => CommandCanceled(connection, command, string.Empty, context, executeMethod, commandId, connectionId, startTime, duration, commandSource);

    public void CommandCanceled(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource)
    {
    }

    public Task CommandCanceledAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => CommandCanceledAsync(connection, command, string.Empty, context, executeMethod, commandId, connectionId, startTime, duration, commandSource, cancellationToken);

    public Task CommandCanceledAsync(
        IRelationalConnection connection,
        DbCommand command,
        string logCommandText,
        DbContext? context,
        DbCommandMethod executeMethod,
        Guid commandId,
        Guid connectionId,
        DateTimeOffset startTime,
        TimeSpan duration,
        CommandSource commandSource,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public InterceptionResult DataReaderDisposing(
        IRelationalConnection connection,
        DbCommand command,
        DbDataReader dataReader,
        Guid commandId,
        int recordsAffected,
        int readCount,
        DateTimeOffset startTime,
        TimeSpan duration)
        => default;

    public InterceptionResult DataReaderClosing(
        IRelationalConnection connection,
        DbCommand command,
        DbDataReader dataReader,
        Guid commandId,
        int recordsAffected,
        int readCount,
        DateTimeOffset startTime)
        => default;

    public ValueTask<InterceptionResult> DataReaderClosingAsync(
        IRelationalConnection connection,
        DbCommand command,
        DbDataReader dataReader,
        Guid commandId,
        int recordsAffected,
        int readCount,
        DateTimeOffset startTime)
        => default;

    public bool ShouldLogCommandCreate(DateTimeOffset now)
        => true;

    public bool ShouldLogCommandExecute(DateTimeOffset now)
        => true;

    public bool ShouldLogDataReaderClose(DateTimeOffset now)
        => true;

    public bool ShouldLogDataReaderDispose(DateTimeOffset now)
        => true;
}
