using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Services.Interfaces;

namespace CJDBelegungsplaner.Domain.Services;

public class DataConnectionService : IDataConnectionService
{
    private readonly AppDbContext _context;

    public DataConnectionService(AppDbContext context)
    {
        _context = context;
    }

    public bool DatabaseCanConnect()
    {
        return _context.Database.CanConnect();
    }
}