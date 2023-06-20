namespace CJDBelegungsplaner.Domain.Results;

public class BedDataServiceResultKind : DataServiceResultKind
{
    public static readonly int NameAlreadyExists = Counter++;

    public BedDataServiceResultKind(int kind) : base(kind)
    {
    }
}
