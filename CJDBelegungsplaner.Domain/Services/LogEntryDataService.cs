using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class LogEntryDataService : ILogEntryDataService
{
    private readonly AppDbContext _context;
    private readonly IGenericDataService<LogEntry> _dataService;

    public LogEntryDataService(AppDbContext context, IGenericDataService<LogEntry> dataService)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task<Result<DataServiceResultKind, LogEntry>> CreateAsync(LogEntry entity)
    {
        return await _dataService.CreateAsync(entity);
    }

    public async Task<Result<DataServiceResultKind, LogEntry>> UpdateAsync(int id, LogEntry entity)
    {
        return await _dataService.UpdateAsync(id, entity);
    }

    public async Task<Result<DataServiceResultKind>> DeleteAsync(int id)
    {
        return await _dataService.DeleteAsync(id);
    }

    public async Task<Result<DataServiceResultKind, LogEntry>> GetAsync(int id)
    {
        return await _dataService.GetOneAsync(
            async () => await _context.LogEntries
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id));
    }

    public async Task<Result<DataServiceResultKind, ICollection<LogEntry>>> GetAllAsync()
    {
        return await _dataService.GetListAsync(
            async () => await _context.LogEntries
            .Include(l => l.User)
            .ToListAsync());
    }
}
