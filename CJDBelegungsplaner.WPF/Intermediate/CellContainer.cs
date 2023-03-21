namespace CJDBelegungsplaner.WPF.Intermediate;

public abstract class CellContainer
{
    public DataColumnWeek DataColumnWeek { get; private set; }

    public int Row { get; protected set; }

    public int Column { get; private set; }

    public CellContainer(DataColumnWeek dataColumnWeek, int row, int column)
    {
        DataColumnWeek = dataColumnWeek;
        Row = row;
        Column = column;
    }
}
