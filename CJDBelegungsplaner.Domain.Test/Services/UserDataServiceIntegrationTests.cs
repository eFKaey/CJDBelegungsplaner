using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using CJDBelegungsplaner.Domain.Services;
using FluentAssertions;
using CJDBelegungsplaner.Domain.Services.Interfaces;

namespace CJDBelegungsplaner.Domain.Test.Services;

public class UserDataServiceIntegrationTests : IDisposable
{
    private readonly DbConnection _dbConnection;
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly List<User> _users = new();

    public UserDataServiceIntegrationTests()
    {
        _dbConnection = new SqliteConnection("Filename=:memory:");
        _dbConnection.Open();

        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_dbConnection)
            .Options;

        using var context = new AppDbContext(_dbContextOptions);

        if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText =
                "CREATE VIEW AllResources AS" + Environment.NewLine +
                "SELECT Name" + Environment.NewLine +
                "FROM Users;";
            viewCommand.ExecuteNonQuery();
        }

        _users = Seed(context);
    }

    private AppDbContext CreateContext() => new(_dbContextOptions);

    public void Dispose() => _dbConnection.Dispose();

    private List<User> Seed(AppDbContext context)
    {
        int userAmount = 3;
        int logEntriesPerUser = 2;

        List<User> users = new();

        for (int u = 0; u < userAmount; u++)
        {
            var user = new User
            {
                Name = $"user{u + 1}",
                PasswordHash = $"hash{u + 1}",
                Role = Role.Standard,
                LogEntries = new List<LogEntry>()
            };

            for (int l = 0; l < logEntriesPerUser; l++)
            {
                user.LogEntries.Add(new LogEntry { Action = $"action{u + 1}{l + 1}" });
            }

            users.Add(user);
        }

        context.AddRange(users);

        context.SaveChanges();

        return users;
    }

    private IUserDataService GetDataService(AppDbContext context)
    {
        var genericDataService = new GenericDataService<User>(
            context,
            new HandleDataExceptionService());
        return new UserDataService(context, genericDataService);
    }

    [Fact]
    public void GetAll()
    {
        // Arange
        using var context = CreateContext();
        var dataService = GetDataService(context);

        // Act
        ICollection<User>? actualUsers = dataService.GetAllAsync().Result.Content;

        // Assert
        actualUsers
            .Should()
            // darf nicht das Selbe sein (nicht die selben Referenzen)
            .NotContain(_users)
            .And
            // aber muss das Gleiche sein (aber der selbe Content)
            .BeEquivalentTo(_users, options =>
                // Ausschließen der "Cyclic reference" User
                options.For(u => u.LogEntries)
                .Exclude(e => e.User));
    }

    [Fact]
    public void Get()
    {
        // Arange
        using var context = CreateContext();
        var dataService = GetDataService(context);

        // Act
        User? actualUser = dataService.GetAsync(_users.First().Id).Result.Content;

        // Assert
        actualUser
            .Should()
            .NotBe(_users.First())
            .And
            .BeEquivalentTo(_users.First(), options =>
                options.For(u => u.LogEntries)
                .Exclude(e => e.User));
    }

    [Fact]
    public void GetUserByUserName()
    {
        // Arange
        using var context = CreateContext();
        var dataService = GetDataService(context);

        // Act
        User? actualUser = dataService.GetByUserNameAsync(_users.First().Name).Result.Content;

        // Assert
        actualUser
            .Should()
            .NotBe(_users.First())
            .And
            .BeEquivalentTo(_users.First(), options =>
                options.For(u => u.LogEntries)
                .Exclude(e => e.User));
    }

    [Fact]
    public void Create()
    {
        // Arange
        var expectedUser = new User
        {
            Name = $"userNew",
            PasswordHash = $"hashNew",
            Role = Role.SuperAdmin,
            LogEntries = new List<LogEntry>()
        };
        expectedUser.LogEntries.Add(new LogEntry { Action = $"actionNew1" });

        using (var contextFirst = CreateContext())
        {
            var dataService = GetDataService(contextFirst);

        // Act
            dataService.CreateAsync(expectedUser);
        }

        using var contextSecond = CreateContext();
        User? actualUser = contextSecond.Users
            .Include(u => u.LogEntries)
            .FirstOrDefault(u => u.Name == expectedUser.Name);

        // Assert
        actualUser
            .Should()
            .NotBe(expectedUser)
            .And
            .BeEquivalentTo(expectedUser, options =>
                options.For(u => u.LogEntries)
                .Exclude(e => e.User));
    }

    [Fact]
    public void Update()
    {
        // Arange
        User expectedUser = _users.First();
        expectedUser.Name = expectedUser.Name + "updated";
        expectedUser.PasswordHash = expectedUser.PasswordHash + "updated";
        expectedUser.Role = Role.Admin;

        using (var contextFirst = CreateContext())
        {
            var dataService = GetDataService(contextFirst);

        // Act
            dataService.UpdateAsync(expectedUser.Id, expectedUser);
        }

        using var contextSecond = CreateContext();
        User? actualUser = contextSecond.Users
            .Include(u => u.LogEntries)
            .FirstOrDefault(u => u.Id == _users.First().Id);

        // Assert
        actualUser
            .Should()
            .NotBe(expectedUser)
            .And
            .BeEquivalentTo(expectedUser, options =>
                options.For(u => u.LogEntries)
                .Exclude(e => e.User));
    }

    [Fact]
    public void Delete()
    {
        // Arange
        using (var contextFirst = CreateContext())
        {
            User user = contextFirst.Users
                .Include(u => u.LogEntries)
                .FirstOrDefault(u => u.Id == _users.First().Id)!;
            var dataService = GetDataService(contextFirst);

        // Act
            dataService.DeleteAsync(user.Id);
        }

        using var contextSecond = CreateContext();
        User? actualUser = contextSecond.Users.FirstOrDefault(u => u.Id == _users.First().Id);   

        // Assert
        actualUser.Should().BeNull();
    }
}