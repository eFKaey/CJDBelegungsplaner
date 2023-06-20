using CJDBelegungsplaner.Domain.Models;
using FluentAssertions;
using Microsoft.VisualBasic;

namespace CJDBelegungsplaner.Domain.Test.Models;

public class UserUnitTests
{
    private readonly User _user;

    public UserUnitTests() 
    {
        _user = new User
        {
            Id = 1,
            Name = "name",
            PasswordHash = "hase value",
            IsLoggedIn = false,
            LastLogin = new DateTime(1111, 1, 1),
            LastLogout = new DateTime(2222, 2, 2),
            Role = Role.None,
            LogEntries = new List<LogEntry>() { new LogEntry() },
        };
    }

    [Fact]
    public void Clone_ReturnNewUserInstanceWithIdenticalContent_WhenCalled()
    {
        /// Arange
        User user = _user;

        /// Act
        User userCloned = user.Clone();

        /// Assert
        userCloned.Should()
            .NotBeSameAs(user)
            .And
            .BeEquivalentTo(user);
    }

    [Fact]
    public void CopyValuesTo_FillsPropertiesWithValuesFromGivenUser_WhenUserIsNotNull()
    {
        /// Arange
        User user = _user;
        User userCloned = user.Clone();
        user.Name = "new name";
        user.PasswordHash = "new hash value";
        user.IsLoggedIn = true;
        user.LastLogin = new DateTime(1111, 1, 2);
        user.LastLogout = new DateTime(2222, 2, 3);
        user.Role = Role.Admin;
        user.LogEntries = new List<LogEntry>();

        /// Act
        userCloned.CopyValuesTo(user);

        /// Assert
        user.Should()
            .NotBeSameAs(userCloned)
            .And
            .BeEquivalentTo(userCloned);
    }
}
