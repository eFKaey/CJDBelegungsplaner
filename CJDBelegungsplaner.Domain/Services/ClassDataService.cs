using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services;

public class ClassDataService : IClassDataService
{
    private readonly AppDbContext _context;
    private readonly IGenericDataService<Class> _dataService;

    public ClassDataService(AppDbContext context, IGenericDataService<Class> dataService)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task<Result<ClassDataServiceResultKind, ICollection<Class>>> GetAllAsync()
    {
        Result<ClassDataServiceResultKind, ICollection<Class>> response = new();

        Result<DataServiceResultKind, ICollection<Class>> result = await _dataService.GetListAsync(
            async () => await _context.Classes
            .Include(c => c.Guests)
            .Include(c => c.Reservations)
            .OrderBy(c => c.Name)
            .ToListAsync());

        return response.PassOn(result);
    }

    public async Task<Result<ClassDataServiceResultKind, ICollection<Class>>> GetAllWithReservationsAsync()
    {
        Result<ClassDataServiceResultKind, ICollection<Class>> response = new();

        Result<DataServiceResultKind, ICollection<Class>> result = await _dataService.GetListAsync(
            async () => await _context.Classes
            .Where(g => g.Reservations.Count != 0)
            .Include(g => g.Guests)
            .Include(g => g.Reservations)
            .OrderBy(c => c.Name)
            .ToListAsync());

        return response.PassOn(result);
    }

    public async Task<Result<ClassDataServiceResultKind, Class>> GetAsync(int id)
    {
        Result<ClassDataServiceResultKind, Class> response = new();

        Result<DataServiceResultKind, Class> result = await _dataService.GetOneAsync(
            async () => await _context.Classes
            .Include(c => c.Guests)
            .Include(c => c.Reservations)
            .FirstOrDefaultAsync((c) => c.Id == id));

        return response.PassOn(result);
    }

    public async Task<Result<ClassDataServiceResultKind, Class>> CreateAsync(Class entity)
    {
        Result<ClassDataServiceResultKind, Class> response = new();

        Result<DataServiceResultKind, Class> result = await _dataService.CreateAsync(entity);

        if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
            && result.IsMatter
            && result.Matter!.Contains("Classes.Name"))
        {
            return response.Failure(ClassDataServiceResultKind.NameAlreadyExists, entity);
        }

        return response.PassOn(result);
    }

    public async Task<Result<ClassDataServiceResultKind, Class>> UpdateAsync(int id, Class entity)
    {
        Result<ClassDataServiceResultKind, Class> response = new();

        Result<DataServiceResultKind, Class> result = await _dataService.UpdateAsync(id, entity);

        if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
            && result.IsMatter
            && result.Matter!.Contains("Classes.Name"))
        {
            return response.Failure(ClassDataServiceResultKind.NameAlreadyExists, entity);
        }

        return response.PassOn(result);
    }

    public async Task<Result<ClassDataServiceResultKind>> DeleteAsync(int id)
    {
        return new Result<ClassDataServiceResultKind>().PassOn(await _dataService.DeleteAsync(id));
    }
}
