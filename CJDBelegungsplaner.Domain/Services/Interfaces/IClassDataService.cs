using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IClassDataService : IDataService<ClassDataServiceResultKind, Class>
{
    Task<Result<ClassDataServiceResultKind, ICollection<Class>>> GetAllWithReservationsAsync();
}
