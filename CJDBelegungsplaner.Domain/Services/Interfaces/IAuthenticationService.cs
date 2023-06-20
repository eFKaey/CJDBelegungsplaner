using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.Domain.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<AuthenticationResultKind, User>> RegisterAsync(string userName, Role role, string password, string confirmPassword);

    Task<Result<AuthenticationResultKind, User>> LoginAsync(string userName, string password);

    Task<Result<AuthenticationResultKind>> LogoutAsync(User user);

    Task<Result<AuthenticationResultKind, User>> UpdateAsync(User orginalUser, string userName, Role role);

    Task<Result<AuthenticationResultKind, User>> UpdateAsync(User orginalUser, string userName, Role role, string password, string confirmPassword);

    Task<Result<AuthenticationResultKind, User>> UpdateAsync(User orginalUser, string password, string confirmPassword);
}
