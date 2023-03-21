namespace CJDBelegungsplaner.Domain.Results
{
    public class CompanyDataServiceResultKind : DataServiceResultKind
    {
        public static readonly int NameAlreadyExists = Counter++;
        public CompanyDataServiceResultKind(int kind) : base(kind)
        {
        }
    }
}
