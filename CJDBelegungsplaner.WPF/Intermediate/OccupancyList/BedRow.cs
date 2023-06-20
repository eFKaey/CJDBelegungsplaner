using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels;
using System.Collections.ObjectModel;

namespace CJDBelegungsplaner.WPF.Intermediate.OccupancyList;

public class BedRow
{
    public Bed Bed { get; set; }

    public ObservableCollection<Day> Days { get; set; } = new();
}
