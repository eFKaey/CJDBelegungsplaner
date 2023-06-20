using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface ILogEntryDataService : IDataService<DataServiceResultKind, LogEntry>
{
    /// <summary>
    /// Löscht alle Log-Einträge, die älter sind als das übergebene Datum.
    /// </summary>
    /// <param name="deadLine"></param>
    /// <returns></returns>
    Task<Result<DataServiceResultKind>> DeleteAllBeforeAsync(DateTime deadLine);
}