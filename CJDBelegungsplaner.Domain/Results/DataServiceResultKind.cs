namespace CJDBelegungsplaner.Domain.Results;

/// <summary>
/// Für Dokumentation siehe: <see cref="ResultKind"/>
/// </summary>
public class DataServiceResultKind : ResultKind
{
    public static readonly int Success = Counter++;
    public static readonly int NoDatabaseConnection = Counter++;
    public static readonly int DatabaseIsLocked = Counter++;
    public static readonly int NotFoundBySearchTerm = Counter++;
    public static readonly int UniqueConstraintFailed = Counter++;
    public static readonly int UpdateConcurrencyFailure = Counter++;
    public static readonly int DatabaseAccessFailed = Counter++;
    public static readonly int AlreadyExists = Counter++;
    public static readonly int DoesntExists = Counter++;
    public static readonly int Failed = Counter++;

    public DataServiceResultKind(int kind) : base(kind)
    {
    }
}
