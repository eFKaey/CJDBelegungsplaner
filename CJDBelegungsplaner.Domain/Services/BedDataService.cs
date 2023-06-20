using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class BedDataService : IBedDataService
{
    private readonly AppDbContext _context;
    private readonly IGenericDataService<Bed> _dataService;

    public BedDataService(AppDbContext context, IGenericDataService<Bed> dataService)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task<Result<BedDataServiceResultKind, Bed>> CreateAsync(Bed entity)
    {
        Result<BedDataServiceResultKind, Bed> response = new();

        Result<DataServiceResultKind, Bed> result = await _dataService.CreateAsync(entity);

        if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
            && result.IsMatter
            && result.Matter!.Contains("Beds.Name"))
        {
            return response.Failure(BedDataServiceResultKind.NameAlreadyExists, entity);
        }

        return response.PassOn(result);
    }

    public async Task<Result<BedDataServiceResultKind>> DeleteAsync(int id)
    {
        return new Result<BedDataServiceResultKind>().PassOn(await _dataService.DeleteAsync(id));
    }

    public async Task<Result<BedDataServiceResultKind, ICollection<Bed>>> GetAllAsync()
    {
        Result<BedDataServiceResultKind, ICollection<Bed>> response = new();

        Result<DataServiceResultKind, ICollection<Bed>> result = await _dataService.GetListAsync(
            async () => await _context.Beds
            .Include(b => b.Occupancies)
            .ThenInclude(o => o.Guest)
            .OrderBy(b => b.Name)
            .ToListAsync());

        return response.PassOn(result);
    }

    public async Task<Result<BedDataServiceResultKind, Bed>> GetAsync(int id)
    {
        Result<BedDataServiceResultKind, Bed> response = new();

        Result<DataServiceResultKind, Bed> result = await _dataService.GetOneAsync(
            async () => await _context.Beds
            .Include(b => b.Occupancies)
            .ThenInclude(o => o.Guest)
            .OrderBy(b => b.Name)
            .FirstOrDefaultAsync(g => g.Id == id));

        return response.PassOn(result);
    }

    public async Task<Result<BedDataServiceResultKind, Bed>> UpdateAsync(int id, Bed entity)
    {
        Result<BedDataServiceResultKind, Bed> response = new();

        Result<DataServiceResultKind, Bed> result = await _dataService.UpdateAsync(id, entity);

        if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
            && result.IsMatter
            && result.Matter!.Contains("Beds.Name"))
        {
            return response.Failure(BedDataServiceResultKind.NameAlreadyExists, entity);
        }

        return response.PassOn(result);
    }
}
