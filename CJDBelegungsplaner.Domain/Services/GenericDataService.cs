using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;

namespace CJDBelegungsplaner.Domain.Services;

public class GenericDataService<TModel> : IGenericDataService<TModel> where TModel : EntityObject
{
    private readonly AppDbContext _context;
    private readonly IHandleDataExceptionService _handleDataExceptionService;

    public GenericDataService(AppDbContext context, IHandleDataExceptionService handleDataExceptionService)
    {
        _context = context;
        _handleDataExceptionService = handleDataExceptionService;
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, ICollection<TModel>>> GetAllAsync()
    {
        return await GetListAsync(async () => await _context.Set<TModel>().ToListAsync());
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, ICollection<TModel>>> GetListAsync(Func<Task<ICollection<TModel>>> funcLINQ)
    {
        Result<DataServiceResultKind, ICollection<TModel>> response = new();

        if (!_context.Database.CanConnect())
        {
            return response.Failure(DataServiceResultKind.NoDatabaseConnection);
        }

        ICollection<TModel> entities;

        try
        {
            entities = await funcLINQ();
        }
        catch (Exception exception)
        {
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        return response.Success(DataServiceResultKind.Success, entities);
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, TModel>> GetAsync(int id)
    {
        return await GetOneAsync(async () => await _context.Set<TModel>().FirstOrDefaultAsync((e) => e.Id == id));
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, TModel>> GetOneAsync(Func<Task<TModel>> funcLINQ)
    {
        Result<DataServiceResultKind, TModel> response = new();

        if (!_context.Database.CanConnect())
        {
            return response.Failure(DataServiceResultKind.NoDatabaseConnection);
        }

        TModel? entity;

        try
        {
            entity = await funcLINQ();
        }
        catch (Exception exception)
        {
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        if (entity is null)
        {
            return response.Failure(DataServiceResultKind.NotFoundBySearchTerm);
        }

        return response.Success(DataServiceResultKind.Success, entity);
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, TModel>> CreateAsync(TModel entity)
    {
        Result<DataServiceResultKind, TModel> response = new();

        if (!_context.Database.CanConnect()) {
            return response.Failure(DataServiceResultKind.NoDatabaseConnection); }

        EntityEntry<TModel> result;

        try
        {
            result = await _context.Set<TModel>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            _context.ChangeTracker.Clear();
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        return response.Success(DataServiceResultKind.Success, result.Entity);
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind, TModel>> UpdateAsync(int id, TModel entity)
    {
        Result<DataServiceResultKind, TModel> response = new();

        if (!_context.Database.CanConnect()) {
            return response.Failure(DataServiceResultKind.NoDatabaseConnection); }

        entity.Id = id;

        try
        {
            _context.Set<TModel>().Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            _context.ChangeTracker.Clear();
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        return response.Success(DataServiceResultKind.Success, entity);
    }

    //
    //
    //

    public async Task<Result<DataServiceResultKind>> DeleteAsync(int id)
    {
        Result<DataServiceResultKind> response = new();

        if (!_context.Database.CanConnect()) {
            return response.Failure(DataServiceResultKind.NoDatabaseConnection); }
        
        TModel? entity = await _context.Set<TModel>().FirstOrDefaultAsync((e) => e.Id == id);

        if (entity is null) {
            return response.Failure(DataServiceResultKind.NotFoundBySearchTerm); }

        try
        {
            _context.Set<TModel>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            _context.ChangeTracker.Clear();
            return response.PassOn(_handleDataExceptionService.Handle(exception));
        }

        return response.Success(DataServiceResultKind.Success);
    }
}
