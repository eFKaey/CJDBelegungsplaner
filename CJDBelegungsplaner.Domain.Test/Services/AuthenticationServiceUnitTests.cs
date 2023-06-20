using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Moq;
using SimpleTrader.Domain.Services;

namespace CJDBelegungsplaner.Domain.Test.Services;
public class AuthenticationServiceUnitTests
{
    private readonly AuthenticationService _sut; // sut: System Under Test
    private readonly Mock<IUserDataService> _userDataServiceMock = new Mock<IUserDataService>();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new Mock<IPasswordHasher>();


    public AuthenticationServiceUnitTests()
    {
        _sut = new AuthenticationService(_userDataServiceMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Login_ReturnsSuccessResultWithUser_WhenUserExistsAndPwIsCorrect()
    {
        // Arange
        var userName = "name";
        var password = "pw";
        var user = new User()
        {
            Id = 1,
            Name = userName,
            PasswordHash = "Hash value",
            IsLoggedIn = true,
            LastLogout = DateTime.Now.AddDays(-1),
        };
        Result<DataServiceResultKind, User> returnResult = new(
            true,
            new DataServiceResultKind(DataServiceResultKind.Success),
            user);
        var passwordResult = PasswordVerificationResult.Success;

        _userDataServiceMock.Setup(x => x.GetByUserNameAsync(userName)).ReturnsAsync(returnResult);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user.PasswordHash, password)).Returns(passwordResult);
        _userDataServiceMock.Setup(x => x.UpdateAsync(user.Id, user)).ReturnsAsync(returnResult);

        // Act
        var result = await _sut.LoginAsync(userName, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userName, result.Content.Name);
    }

    [Fact]
    public async Task Login_ReturnsFailureResult_WhenUserNameDoesntExists()
    {
        // Arange
        var userName = It.IsAny<string>();
        Result<DataServiceResultKind, User> returnResult = new(
            false,
            new DataServiceResultKind(DataServiceResultKind.NotFoundBySearchTerm));

        _userDataServiceMock.Setup(x => x.GetByUserNameAsync(userName)).ReturnsAsync(returnResult);

        // Act
        var result = await _sut.LoginAsync(userName, It.IsAny<string>());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AuthenticationResultKind.UserWasNotFound, result.Kind.KindValue);
    }

    [Fact]
    public async Task Login_ReturnsFailureResult_WhenPasswordsDoesntMatch()
    {
        // Arange
        var userName = It.IsAny<string>();
        var password = It.IsAny<string>();
        var user = new User()
        {
            Id = 1,
            Name = userName,
            PasswordHash = It.IsAny<string>(),
            IsLoggedIn = true,
            LastLogout = DateTime.Now.AddDays(-1),
        };
        Result<DataServiceResultKind, User> userDataServiceResultWithFoundUser = new(
            true,
            new DataServiceResultKind(DataServiceResultKind.Success),
            user);
        var passwordResult = PasswordVerificationResult.Failed;
        Result<AuthenticationResultKind, User> expectedResult = new (
            false,
            new AuthenticationResultKind(AuthenticationResultKind.PasswordsDoNotMatch));
        _userDataServiceMock.Setup(x => x.GetByUserNameAsync(userName))
            .ReturnsAsync(userDataServiceResultWithFoundUser);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user.PasswordHash, password))
            .Returns(passwordResult);

        // Act
        var result = await _sut.LoginAsync(userName, password);

        // Assert mit FluentAssertions
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Logout_ReturnsSuccessResult_WhenUpdateSuccessfullyFinishes()
    {
        /// Arange
        var user = new User()
        {
            Id = It.IsInRange(1, int.MaxValue, Moq.Range.Inclusive),
            Name = It.IsAny<string>(),
            PasswordHash = It.IsAny<string>(),
            IsLoggedIn = true,
            LastLogout = DateTime.Now.AddDays(-1),
        };
        Result<DataServiceResultKind, User> returnResult = new(
            true,
            new DataServiceResultKind(DataServiceResultKind.Success),
            user);
        _userDataServiceMock
            .Setup(x => x.UpdateAsync(user.Id, user))
            .ReturnsAsync(returnResult);

        /// Act
        var result = await _sut.LogoutAsync(user);

        /// Assert
        Assert.True(result.IsSuccess);
    }
}
