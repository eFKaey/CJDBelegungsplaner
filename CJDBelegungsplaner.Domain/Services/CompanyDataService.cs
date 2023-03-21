using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.Domain.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CJDBelegungsplaner.Domain.Services
{
    public class CompanyDataService : ICompanyDataService
    {
        private readonly AppDbContext _context;
        private readonly IGenericDataService<Company> _dataService;

        public CompanyDataService(AppDbContext context, IGenericDataService<Company> dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        public async Task<Result<CompanyDataServiceResultKind, Company>> CreateAsync(Company entity)
        {
            Result<CompanyDataServiceResultKind, Company> response = new();

            Result<DataServiceResultKind, Company> result = await _dataService.CreateAsync(entity);

            if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
                && result.IsMatter
                && result.Matter!.Contains("Company.Name"))
            {
                return response.Failure(CompanyDataServiceResultKind.NameAlreadyExists, entity);
            }

            return response.PassOn(result);
        }

        public async Task<Result<CompanyDataServiceResultKind>> DeleteAsync(int id)
        {
            return new Result<CompanyDataServiceResultKind>().PassOn(await _dataService.DeleteAsync(id));
        }

        public async Task<Result<CompanyDataServiceResultKind, Company>> GetAsync(int id)
        {
            Result<CompanyDataServiceResultKind, Company> response = new();

            Result<DataServiceResultKind, Company> result =await _dataService.GetOneAsync(
                async () => await _context.Companies
                .Include(c => c.Guests)
                .FirstOrDefaultAsync((c) => c.Id == id));

            return response.PassOn(result);
        }

        public async Task<Result<CompanyDataServiceResultKind, ICollection<Company>>> GetAllAsync()
        {
            Result<CompanyDataServiceResultKind, ICollection<Company>> respone = new();

            Result<DataServiceResultKind, ICollection<Company>> result = await _dataService.GetListAsync(
                async () => await _context.Companies
                .Include(c => c.Guests)
                .ToListAsync());
            
            return respone.PassOn(result);
        }

        public async Task<Result<CompanyDataServiceResultKind, Company>> UpdateAsync(int id, Company entity)
        {
            Result<CompanyDataServiceResultKind, Company> response = new();

            Result<DataServiceResultKind, Company> result = await _dataService.UpdateAsync(id, entity);

            if (result.Kind! == DataServiceResultKind.UniqueConstraintFailed
                && result.IsMatter
                && result.Matter!.Contains("Company.Name"))
            {
                return response.Failure(CompanyDataServiceResultKind.NameAlreadyExists, entity);
            }

            return response.PassOn(result);
        }
    }
}
