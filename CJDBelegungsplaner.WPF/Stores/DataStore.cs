using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Stores;

public class DataStore
{
    private ObservableCollection<Guest>? _guests;
    private ObservableCollection<Class>? _classes;
    private ObservableCollection<Company>? _companies;
    private ObservableCollection<User>? _users;
    private ObservableCollection<Bed>? _beds;

    private readonly IDataHelperSevice _dataHelperSevice;
    private readonly IGuestDataService _guestDataService;
    private readonly IClassDataService _classDataService;
    private readonly ICompanyDataService _companyDataService;
    private readonly IUserDataService _userDataService;
    private readonly IBedDataService _bedDataService;

    public DataStore(
        IDataHelperSevice dataHelperSevice,
        IGuestDataService guestDataService,
        IClassDataService classDataService,
        ICompanyDataService companyDataService,
        IUserDataService userDataService,
        IBedDataService bedDataService)
    {
        _dataHelperSevice = dataHelperSevice;
        _guestDataService = guestDataService;
        _classDataService = classDataService;
        _companyDataService = companyDataService;
        _userDataService = userDataService;
        _bedDataService = bedDataService;
    }

    public ObservableCollection<Guest> GetGuests(bool forcePull = false)
    {
        if (forcePull || _guests is null)
        {
            _guests = _dataHelperSevice.GetCollection(_guestDataService.GetAllAsync);
        }

        return _guests;
    }

    public async Task<ObservableCollection<Guest>> GetGuestsAsync(bool forcePull = false)
    {
        if (forcePull || _guests is null)
        {
            _guests = await _dataHelperSevice.GetCollectionAsync(_guestDataService.GetAllAsync);
        }

        return _guests;
    }

    public ObservableCollection<Class> GetClasses(bool forcePull = false)
    {
        if (forcePull || _classes is null)
        {
            _classes = _dataHelperSevice.GetCollection(_classDataService.GetAllAsync);
        }

        return _classes;
    }

    public async Task<ObservableCollection<Class>> GetClassesAsync(bool forcePull = false)
    {
        if (forcePull || _classes is null)
        {
            _classes = await _dataHelperSevice.GetCollectionAsync(_classDataService.GetAllAsync);
        }

        return _classes;
    }

    public ObservableCollection<Company> GetCompanies(bool forcePull = false)
    {
        if (forcePull || _companies is null)
        {
            _companies = _dataHelperSevice.GetCollection(_companyDataService.GetAllAsync);
        }

        return _companies;
    }

    public async Task<ObservableCollection<Company>> GetCompaniesAsync(bool forcePull = false)
    {
        if (forcePull || _companies is null)
        {
            _companies = await _dataHelperSevice.GetCollectionAsync(_companyDataService.GetAllAsync);
        }

        return _companies;
    }

    public ObservableCollection<User> GetUsers(bool forcePull = false)
    {
        if (forcePull || _users is null)
        {
            _users = _dataHelperSevice.GetCollection(_userDataService.GetAllAsync);
        }

        return _users;
    }

    public async Task<ObservableCollection<User>> GetUsersAsync(bool forcePull = false)
    {
        if (forcePull || _users is null)
        {
            _users = await _dataHelperSevice.GetCollectionAsync(_userDataService.GetAllAsync);
        }

        return _users;
    }

    public ObservableCollection<Bed> GetBeds(bool forcePull = false)
    {
        if (forcePull || _beds is null)
        {
            _beds = _dataHelperSevice.GetCollection(_bedDataService.GetAllAsync);
        }

        return _beds;
    }

    public async Task<ObservableCollection<Bed>> GetBedsAsync(bool forcePull = false)
    {
        if (forcePull || _beds is null)
        {
            _beds = await _dataHelperSevice.GetCollectionAsync(_bedDataService.GetAllAsync);
        }

        return _beds;
    }
}
