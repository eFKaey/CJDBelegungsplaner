using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IUserDataService : IDataService<DataServiceResultKind, User>
{
    Task<Result<DataServiceResultKind, User>> GetByUserNameAsync(string userName);
}
