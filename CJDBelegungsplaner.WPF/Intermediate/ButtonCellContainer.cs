namespace CJDBelegungsplaner.WPF.Intermediate;

public class ButtonCellContainer : CellContainer
{
    public ButtonCellContainer(DataColumnWeek dataColumnWeek, int row, int column) : base(dataColumnWeek, row, column)
    {
    }

    public void SetRow(int row)
    {
        Row = row;
    }
}