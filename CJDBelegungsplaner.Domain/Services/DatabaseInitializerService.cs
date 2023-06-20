using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class DatabaseInitializerService : IDatabaseInitializerService
{
    private readonly AppDbContext _context;
    private readonly IHandleDataExceptionService _handleDataExceptionService;

    public DatabaseInitializerService(AppDbContext context, IHandleDataExceptionService handleDataExceptionService)
    {
        _context = context;
        _handleDataExceptionService = handleDataExceptionService;
    }

    public Result<DataServiceResultKind> InitializeDatabase()
    {
        Result<DataServiceResultKind> response = new();

        try
        {
            _context.Database.Migrate();
        }
        catch (Exception exception)
        {
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        return response.Success(DataServiceResultKind.Success);
    }
}
