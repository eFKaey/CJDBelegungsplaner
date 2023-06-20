using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class GuestDataService : IGuestDataService
{
    private readonly AppDbContext _context;
    private readonly IGenericDataService<Guest> _dataService;

    public GuestDataService(AppDbContext context, IGenericDataService<Guest> dataService)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task<Result<DataServiceResultKind, Guest>> CreateAsync(Guest entity)
    {
        return await _dataService.CreateAsync(entity);
    }

    public async Task<Result<DataServiceResultKind>> DeleteAsync(int id)
    {
        return await _dataService.DeleteAsync(id);
    }

    public async Task<Result<DataServiceResultKind, Guest>> GetAsync(int id)
    {
        return await _dataService.GetOneAsync(
            async () => await _context.Guests
            .Include(g => g.Class)
            .Include(g => g.Company)
            .Include(g => g.Reservations)
            .Include(g => g.ClassReservations)
            .Include(g => g.LogEntries)
            .FirstOrDefaultAsync(g => g.Id == id));
    }

    public async Task<Result<DataServiceResultKind, ICollection<Guest>>> GetAllAsync()
    {
        return await _dataService.GetListAsync(
            async () => await _context.Guests
            .Include(g => g.Class)
            .Include(g => g.Company)
            .Include(g => g.Reservations)
            .Include(g => g.ClassReservations)
            .Include(g => g.LogEntries)
            .OrderBy(g => g.FirstName)
            .ToListAsync());
    }

    public async Task<Result<DataServiceResultKind, ICollection<Guest>>> GetAllWithoutClassAsync()
    {
        return await _dataService.GetListAsync(
            async () => await _context.Guests
            .Where(g => g.Class == null)
            .Include(g => g.Company)
            .Include(g => g.Reservations)
            .Include(g => g.LogEntries)
            .OrderBy(g => g.FirstName)
            .ToListAsync());
    }

    public async Task<Result<DataServiceResultKind, ICollection<Guest>>> GetAllWithReservationsAsync()
    {
        return await _dataService.GetListAsync(
            async () => await _context.Guests
            .Where(g => g.Reservations.Count != 0)
            .Include(g => g.Class)
            .Include(g => g.Company)
            .Include(g => g.Reservations)
            .Include(g => g.ClassReservations)
            .Include(g => g.LogEntries)
            .OrderBy(g => g.FirstName)
            .ToListAsync());
    }

    public async Task<Result<DataServiceResultKind, Guest>> UpdateAsync(int id, Guest entity)
    {
        return await _dataService.UpdateAsync(id, entity);
    }

}
