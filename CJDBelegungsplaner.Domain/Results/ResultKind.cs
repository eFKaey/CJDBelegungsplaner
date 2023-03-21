using System.Reflection;

namespace CJDBelegungsplaner.Domain.Results;

/// <summary>
/// <para>
/// Eine bastrakte Klasse, die als Basis für ResultKind-Klassen die Funktionalität beinhaltet.
/// Die abgeleiteten ResultKind Klassen sind als Ersatz für Enums gedacht, sollen also ähnlich
/// funktionieren, können aber im Gegensatz zu Enums abgeleitet werden.
/// </para>
/// Anwendung:<br/>
/// Bei abgeleiteten Klassen muss lediglich der Konstruktor implementiert werden. Natürlich
/// müssen wie bei einem Enums die Konstanten(Werte) anegelegt werden.
/// Diese können vom Typ "public static readonly int" sein, bzw. sind als solcher gedacht. Aber
/// vielleicht machen auch andere Sinn. Es sollte klar sein, dass ein in einer Mutter-Klasse
/// angelegter Wert auch in den abgeleiteten Kind-Klassen zur verfügung steht.
/// <br/>
/// Desweiteren kann der Counter der Basisklasse benutzt werden, um die Werte kontinulierlich 
/// über die Klassen hinweg hochzählen zu lassen.
/// <para>
/// Beispiel einer abgeleiteten Klasse:
/// <code>
///     public class DataServiceResultKind : ResultKind
///     {
///         public static readonly int Success = Counter++;
///         public static readonly int NoDatabaseConnection = Counter++;
///         public static readonly int NotFoundBySearchTerm = Counter++;
///         public static readonly int UniqueConstraintFailed = Counter++;
///         public static readonly int AlreadyExists = Counter++;
///         public static readonly int DoesntExists = Counter++;
///         public static readonly int Failed = Counter++;
///         
///         public DataServiceResultKind(int kind) : base(kind)
///         {
///         }
///     }
/// </code>
/// </para>
/// </summary>
public abstract class ResultKind
{
    protected static int Counter = 0;

    public int KindValue { get; }

    public ResultKind(int kind)
    {
        KindValue = kind;
    }

    public bool Equals(int num)
    {
        if (this.KindValue == num)
        {
            return true;
        }
        return false;
    }

    public static bool operator ==(ResultKind obj, int num)
    {
        return obj.Equals(num);
    }
    public static bool operator !=(ResultKind obj, int num)
    {
        return !obj.Equals(num);
    }

    public static bool operator ==(int num, ResultKind obj)
    {
        return obj.Equals(num);
    }
    public static bool operator !=(int num, ResultKind obj)
    {
        return !obj.Equals(num);
    }

    public static implicit operator int(ResultKind k) => k.KindValue;

    public static implicit operator string(ResultKind k) => k.ToString();

    public override string ToString()
    {
        foreach (FieldInfo fieldInfo in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
        {
            if (fieldInfo.FieldType != typeof(int))
            {
                continue;
            }
            if ((int)fieldInfo.GetValue(this) == KindValue)
            {
                return fieldInfo.Name;
            }
        }
        return $"No field found! (KindValue: {KindValue})";
    }
}
