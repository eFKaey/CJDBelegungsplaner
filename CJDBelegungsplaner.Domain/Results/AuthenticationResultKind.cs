namespace CJDBelegungsplaner.Domain.Results;

/// <summary>
/// Für Dokumentation siehe: <see cref="ResultKind"/>
/// </summary>
public class AuthenticationResultKind : DataServiceResultKind
{
    public static readonly int PasswordsDoNotMatch = Counter++;
    public static readonly int PasswordIsToShort = Counter++;
    public static readonly int UserNameAlreadyExists = Counter++;
    public static readonly int UserNameIsToShort = Counter++;
    public static readonly int UserWasNotFound = Counter++;

    public AuthenticationResultKind(int kind) : base(kind)
    {
    }
}