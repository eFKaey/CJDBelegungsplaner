using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<AuthenticationResultKind, User>> Register(string userName, Role role, string password, string confirmPassword);

    Task<Result<AuthenticationResultKind, User>> Login(string userName, string password);

    Task<Result<AuthenticationResultKind>> Logout(User user);

    Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string userName, Role role);

    Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string userName, Role role, string password, string confirmPassword);

    Task<Result<AuthenticationResultKind, User>> Update(User orginalUser, string password, string confirmPassword);
}
