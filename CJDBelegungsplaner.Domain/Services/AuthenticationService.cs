using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using Microsoft.AspNet.Identity;

namespace SimpleTrader.Domain.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserDataService _userDataService;

    /// Nuget: Microsoft.AspNet.Identity.Core
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationService(IUserDataService userDataService, IPasswordHasher passwordHasher)
    {
        _userDataService = userDataService;
        _passwordHasher = passwordHasher;
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind, User>> Login(string userName, string password)
    {
        Result<DataServiceResultKind, User> result = await _userDataService.GetByUserName(userName);

        Result<AuthenticationResultKind, User> response = new();

        if (result.Kind == DataServiceResultKind.NotFoundBySearchTerm)
        {
            return response.Failure(AuthenticationResultKind.UserWasNotFound);
        }

        if (result.IsFailure)
        {
            return response.PassOn(result);
        }

        User storedUser = result.Content!;

        PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(
            storedUser.PasswordHash, password);

        if (passwordResult != PasswordVerificationResult.Success) {
            return response.Failure(AuthenticationResultKind.PasswordsDoNotMatch); }

        User tempUser = storedUser.Clone();

        storedUser.LastLogin = DateTime.Now;
        storedUser.IsLoggedIn = true;
        //storedUser.LogEntries.Add(new LogEntry { Action = "Login.", Created = storedUser.LastLogin });

        result = await _userDataService.UpdateAsync(storedUser.Id, storedUser);

        if (result.Kind == DataServiceResultKind.DatabaseIsLocked)
        {
            tempUser.CopyValuesTo(storedUser);
            return response.Failure(result.Kind, result.Matter, storedUser);
        }

        return response.PassOn(result);
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind>> Logout(User user)
    {
        User tempUser = user.Clone();

        user.LastLogout = DateTime.Now;
        user.IsLoggedIn = false;
        //user.LogEntries.Add(new LogEntry { Action = "Logout.", Created = user.LastLogout });

        Result<DataServiceResultKind, User> result = await _userDataService.UpdateAsync(user.Id, user);

        if (result.IsFailure)
        {
            tempUser.CopyValuesTo(user);
        }

        return new Result<AuthenticationResultKind>().PassOn(result);
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind, User>> Register(string userName, Role role, string password, string confirmPassword)
    {
        Result<AuthenticationResultKind, User> result;

        result = await ValidateUserName(userName, -1);

        if (result.IsFailure) {
            return result; }

        result = ValidatePassword(password, confirmPassword);

        if (result.IsFailure) {
            return result; }

        string hashedPassword = _passwordHasher.HashPassword(password);

        User newUser = new User()
        {
            Name = userName,
            PasswordHash = hashedPassword,
            IsLoggedIn = false,
            LastLogin = DateTime.MinValue,
            LastLogout = DateTime.MinValue,
            Role = role,
            Created = DateTime.Now
        };

        return new Result<AuthenticationResultKind, User>().PassOn(await _userDataService.CreateAsync(newUser));
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string userName, Role role)
    {
        Result<AuthenticationResultKind, User> result = await ValidateUserName(userName, orginalUser.Id);

        if (result.IsFailure) {
            return result;  }

        orginalUser.Name = userName;
        orginalUser.Role = role;

        return new Result<AuthenticationResultKind, User>().PassOn(await _userDataService.UpdateAsync(orginalUser.Id, orginalUser));
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string userName, Role role, string password, string confirmPassword)
    {
        Result<AuthenticationResultKind, User> result;

        result = await ValidateUserName(userName, orginalUser.Id);

        if (result.IsFailure) {
            return result; }

        result = ValidatePassword(password, confirmPassword);

        if (result.IsFailure) {
            return result; }

        string hashedPassword = _passwordHasher.HashPassword(password);

        orginalUser.Name = userName;
        orginalUser.PasswordHash = hashedPassword;
        orginalUser.Role = role;

        return new Result<AuthenticationResultKind, User>().PassOn(await _userDataService.UpdateAsync(orginalUser.Id, orginalUser));
    }

    //
    //
    //

    public async Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string password, string confirmPassword)
    {
        Result<AuthenticationResultKind, User> result;

        result = ValidatePassword(password, confirmPassword);

        if (result.IsFailure)
        {
            return result;
        }

        string hashedPassword = _passwordHasher.HashPassword(password);

        orginalUser.PasswordHash = hashedPassword;

        return new Result<AuthenticationResultKind, User>().PassOn(await _userDataService.UpdateAsync(orginalUser.Id, orginalUser));
    }

    //
    //
    //

    private async Task<Result<AuthenticationResultKind, User>> ValidateUserName(string userName, int id)
    {
        Result<AuthenticationResultKind, User> response = new();

        if (userName.Length < 2) {
            return response.Failure(AuthenticationResultKind.UserNameIsToShort); }

        Result<DataServiceResultKind, User> result = await _userDataService.GetByUserName(userName);

        if (result.IsSuccess && id != result.Content?.Id)
        {
            return response.Failure(AuthenticationResultKind.UserNameAlreadyExists);
        }
        else if (result.Kind! == DataServiceResultKind.NotFoundBySearchTerm) 
        {
            return response.Success(AuthenticationResultKind.Success);
        }

        return new Result<AuthenticationResultKind, User>().PassOn(result);
    }

    //
    //
    //

    private Result<AuthenticationResultKind, User> ValidatePassword(string password, string confirmPassword)
    {
        Result<AuthenticationResultKind, User> response = new();

        if (password != confirmPassword) {
            return response.Failure(AuthenticationResultKind.PasswordsDoNotMatch); }

        if (password.Length < 3) {
            return response.Failure(AuthenticationResultKind.PasswordIsToShort); }

        return response.Success(AuthenticationResultKind.Success);
    }
}
