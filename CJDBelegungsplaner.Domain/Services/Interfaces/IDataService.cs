using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IDataService<TResultKind, TModel> 
    where TResultKind : ResultKind
    where TModel : EntityObject
{
    Task<Result<TResultKind, ICollection<TModel>>> GetAllAsync();

    Task<Result<TResultKind, TModel>> GetAsync(int id);

    Task<Result<TResultKind, TModel>> CreateAsync(TModel entity);

    Task<Result<TResultKind, TModel>> UpdateAsync(int id, TModel entity);

    Task<Result<TResultKind>> DeleteAsync(int id);
}
