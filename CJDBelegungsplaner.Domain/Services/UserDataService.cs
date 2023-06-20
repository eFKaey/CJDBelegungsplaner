using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class UserDataService : IUserDataService
{
    private readonly AppDbContext _context;
    private readonly IGenericDataService<User> _dataService;

    public UserDataService(AppDbContext context, IGenericDataService<User> dataService)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task<Result<DataServiceResultKind, ICollection<User>>> GetAllAsync()
    {
        return await _dataService.GetListAsync(
            async () => await _context.Users
            .Include(u => u.LogEntries)
            .ToListAsync());
    }

    public async Task<Result<DataServiceResultKind, User>> GetAsync(int id)
    {
        return await _dataService.GetOneAsync(
            async () => await _context.Users
            .Include(u => u.LogEntries)
            .FirstOrDefaultAsync((u) => u.Id == id));
    }

    public async Task<Result<DataServiceResultKind, User>> GetByUserNameAsync(string userName)
    {
        return await _dataService.GetOneAsync(
            async () => await _context.Users
            .Include(u => u.LogEntries)
            .FirstOrDefaultAsync((u) => u.Name == userName));
    }

    public async Task<Result<DataServiceResultKind, User>> CreateAsync(User entity)
    {
        return await _dataService.CreateAsync(entity);
    }

    public async Task<Result<DataServiceResultKind, User>> UpdateAsync(int id, User entity)
    {
        return await _dataService.UpdateAsync(id, entity);
    }

    public async Task<Result<DataServiceResultKind>> DeleteAsync(int id)
    {
        return await _dataService.DeleteAsync(id);
    }
}
