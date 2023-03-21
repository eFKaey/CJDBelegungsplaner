namespace CJDBelegungsplaner.Domain.Results;

/// <summary>
/// Für Dokumentation siehe: <see cref="ResultKind"/>
/// </summary>
public class ClassDataServiceResultKind : DataServiceResultKind
{
    public static readonly int NameAlreadyExists = Counter++;

    public ClassDataServiceResultKind(int kind) : base(kind)
    {
    }
}