using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IGuestDataService : IDataService<DataServiceResultKind, Guest>
{
    Task<Result<DataServiceResultKind, ICollection<Guest>>> GetAllWithoutClassAsync();

    Task<Result<DataServiceResultKind, ICollection<Guest>>> GetAllWithReservationsAsync();
}