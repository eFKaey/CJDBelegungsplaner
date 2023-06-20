using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

/// <summary>
/// Der IGenericDataService erweitert die standard CRUD-Opererationen des IDataService um dynamische Abfragen,
/// deren LINQ per Parameter übergeben wird. Üblicherweise übernimmt der GenericDataService auch das
/// Exception Handling und gibt entsprechende ResultKinds zurück.
/// </summary>
/// <typeparam name="TModel"></typeparam>
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

    /// <summary>
    /// Anahnd der übergebenen LINQ Funktion werden die Datensätze gelöscht.
    /// </summary>
    /// <param name="funcLINQ"></param>
    /// <returns></returns>
    Task<Result<DataServiceResultKind>> DeleteRangeAsync(Func<Task<ICollection<TModel>>> funcLINQ);
}
