using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.Domain.Services.Interfaces
{
    public interface ICompanyDataService : IDataService<CompanyDataServiceResultKind, Company>
    {
    }
}
