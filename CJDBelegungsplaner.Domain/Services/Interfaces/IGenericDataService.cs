using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IGenericDataService<TModel> : IDataService<DataServiceResultKind, TModel>
    where TModel : EntityObject
{
    /// <summary>
    /// Mittels der übergebenen LINQ Funktion werden die Datensätze als Liste zurückgegeben.
    /// </summary>
    /// <param name="funcLINQ"></param>
    /// <returns></returns>
    Task<Result<DataServiceResultKind, ICollection<TModel>>> GetListAsync(Func<Task<ICollection<TModel>>> funcLINQ);

    /// <summary>
    /// Mittels der übergebenen LINQ Funktion wird der Datensatz zurückgegeben.
    /// </summary>
    /// <param name="funcLINQ"></param>
    /// <returns></returns>
    Task<Result<DataServiceResultKind, TModel>> GetOneAsync(Func<Task<TModel>> funcLINQ);
}
