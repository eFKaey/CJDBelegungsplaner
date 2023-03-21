using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Services.Interfaces;

public interface IDataHelperSevice
{
    /// <summary>
    /// Ruft alle Daten mittels der gegebenen Funktion aus der Datenbank ab.
    /// </summary>
    /// <typeparam name="TResultKind"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    ObservableCollection<TModel>? GetCollection<TResultKind, TModel>(Func<Task<Result<TResultKind, ICollection<TModel>>>> func)
        where TResultKind : DataServiceResultKind
        where TModel : EntityObject;

    /// <summary>
    /// Ruft alle Daten mittels der gegebenen Funktion aus der Datenbank ab.
    /// </summary>
    /// <typeparam name="TResultKind"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="func"></param>
    /// <param name="noAsync"></param>
    /// <returns></returns>
    Task<ObservableCollection<TModel>?> GetCollectionAsync<TResultKind, TModel>(Func<Task<Result<TResultKind, ICollection<TModel>>>> func, bool noAsync = false)
        where TResultKind : DataServiceResultKind
        where TModel : EntityObject;

    /// <summary>
    /// Schaut per Reflection in die Elemente der übergebenen ObservableCollection und gibt die Namen als Liste zurück.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="entities"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public List<string> GetNames<TModel>(ObservableCollection<TModel> entities)
        where TModel : EntityObject;
}
