using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Services;

public class DataHelperSevice : IDataHelperSevice
{
    private readonly IHandleResultService _handleResultService;
    private readonly ProgressDialogViewModelStore _progressDialogViewModelStore;

    public DataHelperSevice(IHandleResultService handleResultService, ProgressDialogViewModelStore progressDialogViewModelStore)
    {
        _handleResultService = handleResultService;
        _progressDialogViewModelStore = progressDialogViewModelStore;
    }

    //
    //
    //

    public ObservableCollection<TModel>? GetCollection<TResultKind, TModel>(Func<Task<Result<TResultKind, ICollection<TModel>>>> func)
        where TResultKind : DataServiceResultKind
        where TModel : EntityObject
    {
        return GetCollectionAsync(func, true).Result;
    }

    //
    //
    //

    public async Task<ObservableCollection<TModel>?> GetCollectionAsync<TResultKind, TModel>(Func<Task<Result<TResultKind, ICollection<TModel>>>> func, bool noAsync)
        where TResultKind : DataServiceResultKind
        where TModel : EntityObject
    {
        Result<TResultKind, ICollection<TModel>> result;

        if (noAsync)
        {
            result = func().Result;
        }
        else
        {
            result = await Task.Run(() => func());
        }

        if (result.Kind is null)
        {
            throw new NullReferenceException(nameof(result.Kind));
        }

        bool isFailure = _handleResultService.Handle(result);

        if (result.Kind == DataServiceResultKind.NoDatabaseConnection)
        {
            _progressDialogViewModelStore.CheckDatabaseConnection();
        }

        if (isFailure)
        {
            return null;
        }

        if (result.Content is null)
        {
            throw new NullReferenceException(nameof(result.Content));
        }

        return new ObservableCollection<TModel>(result.Content);
    }

    //
    //
    //

    public List<string> GetNames<TModel>(ObservableCollection<TModel> entities)
        where TModel : EntityObject
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        var names = new List<string>();

        foreach (TModel entity in entities)
        {
            PropertyInfo? propertyInfo = entity.GetType().GetProperty("Name");

            if (propertyInfo is null)
            {
                break;
            }

            object? value = propertyInfo.GetValue(entity, null);

            if (value is null || value.GetType() != typeof(string))
            {
                continue;
            }

            names.Add((string)value);
        }

        return names;
    }
}
