using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class HandleDataExceptionService : IHandleDataExceptionService
{
    public Result<DataServiceResultKind> Handle(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        Result<DataServiceResultKind> response = new();

        if (exception is DbUpdateConcurrencyException)
        {
            return response.Failure(DataServiceResultKind.UpdateConcurrencyFailure, exception.Message);
        }
        if (exception is SqliteException)
        {
            return response.Failure(HandleSqliteException((SqliteException)exception), getMessages(exception));
        }

        if (exception is not DbUpdateException dbUpdateException || dbUpdateException.InnerException is null)
        {
            return response.Failure(DataServiceResultKind.Failed, getMessages(exception));
        }

        if (dbUpdateException.InnerException is SqliteException sqliteException)
        {
            return response.Failure(HandleSqliteException(sqliteException), getMessages(dbUpdateException));
        }

        return response.Failure(DataServiceResultKind.DatabaseAccessFailed, getMessages(dbUpdateException));
    }

    private int HandleSqliteException(SqliteException sqliteException)
    {
        switch (sqliteException.SqliteExtendedErrorCode)
        {
            case 5:  // Unique constraint error
                return DataServiceResultKind.DatabaseIsLocked;
            case 2067:  // Unique constraint error
                return DataServiceResultKind.UniqueConstraintFailed;
            // Weitere Error Codes unter: https://www.sqlite.org/rescode.html
            default:
                return DataServiceResultKind.Failed;
        }
    }

    private string getMessages(Exception exception)
    {
        string message = "";

        while (true)
        {
            message += exception.Message;

            if (exception.InnerException is null)
            {
                break;
            }

            message += "\n\n";
            exception = exception.InnerException;
        }

        return message;
    }
}
