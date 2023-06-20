using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.Domain.Services.Interfaces
{
    public interface IDatabaseInitializerService
    {
        Result<DataServiceResultKind> InitializeDatabase();
    }
}