using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IHandleDataExceptionService
{
    Result<DataServiceResultKind> Handle(Exception exception);
}
